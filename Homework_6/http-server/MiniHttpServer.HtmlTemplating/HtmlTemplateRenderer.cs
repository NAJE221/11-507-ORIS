// Подключение пространства имён для работы с IEnumerable (foreach по коллекциям)

using System.Collections;
using System.Text;
using MiniHttpServer.HtmlTemplating.Abstractions;
using MiniHttpServer.HtmlTemplating.Expressions.Nodes;
using MiniHttpServer.HtmlTemplating.Expressions.Operands;
// Подключение StringBuilder для эффективной конкатенации строк
// Подключение абстракций: ILexer, IParser, IHtmlTemplateRenderer, Node, Operand
// Подключение узлов AST: TextNode, VariableNode, IfNode, ForeachNode
// Подключение операндов: PathOperand, LiteralOperand

// Пространство имён библиотеки
namespace MiniHttpServer.HtmlTemplating;

/// <summary>
/// Реализация HTML-шаблонизатора: выполняет лексический анализ,
/// синтаксический разбор и рендеринг шаблона в готовую разметку.
/// </summary>
/// <param name="lexer">Лексический анализатор, разбивающий шаблон на токены.</param>
/// <param name="parser">Синтаксический анализатор, строящий дерево узлов.</param>
public class HtmlTemplateRenderer(ILexer lexer, IParser parser) : IHtmlTemplateRenderer
{
    /// <summary>
    /// Массив числовых типов, используемых для проверки,
    /// являются ли операнды числовыми перед сравнением.
    /// </summary>
    private static readonly Type[] NumericTypes =
    [
        // Целочисленные типы
        typeof(byte),
        typeof(short),
        typeof(int),
        typeof(long),
        // Типы с плавающей точкой
        typeof(float),
        typeof(double),
        // Десятичный тип
        typeof(decimal)
    ];

    /// <inheritdoc />
    public string RenderFromString<T>(string htmlTemplate, T dataModel)
    {
        // Шаг 1: разбиваем шаблон на токены с помощью лексера
        var resultLexer = lexer.Tokenize(htmlTemplate);

        // Шаг 2: преобразуем токены в дерево узлов (AST) с помощью парсера
        var resultParser = parser.Parse(resultLexer);

        // Шаг 3: создаём пустой scope для переменных цикла
        Dictionary<string, object> scope = new();

        // Шаг 4: рендерим дерево узлов и возвращаем результат
        return RenderBlock(resultParser, dataModel, scope);
    }

    /// <inheritdoc />
    public string RenderFromFile<T>(string filePath, T dataModel)
    {
        // Проверяем, что входные параметры не null
        if (filePath is null || dataModel is null)
            throw new NullReferenceException("В параметры передан null!");

        // Проверяем существование файла шаблона
        CheckFileExists(filePath);

        // Открываем файл для чтения (using гарантирует закрытие потока)
        using var stream = new StreamReader(filePath);

        // Читаем всё содержимое файла в строку
        var content = stream.ReadToEnd();

        // Делегируем рендеринг методу RenderFromString
        return RenderFromString(content, dataModel);
    }

    /// <inheritdoc />
    public string RenderToFile<T>(string inputFilePath, string outputFilePath, T dataModel)
    {
        // Проверяем, что входные параметры не null
        if (inputFilePath is null || outputFilePath is null || dataModel is null)
            throw new NullReferenceException("В параметры передан null!");

        // Проверяем существование входного файла
        CheckFileExists(inputFilePath);

        // Рендерим шаблон из входного файла
        var content = RenderFromFile(inputFilePath, dataModel);

        // Открываем выходной файл для записи
        using StreamWriter streamWriter = new(outputFilePath);

        // Записываем сгенерированную разметку
        streamWriter.Write(content);

        // Возвращаем сгенерированную разметку
        return content;
    }

    /// <summary>
    /// Рендерит список узлов AST в строку.
    /// </summary>
    /// <typeparam name="T">Тип модели данных.</typeparam>
    /// <param name="listNodes">Список узлов для рендеринга.</param>
    /// <param name="dataModel">Объект модели данных.</param>
    /// <param name="scope">Словарь переменных цикла (scope).</param>
    /// <returns>Сгенерированная строка.</returns>
    private string RenderBlock<T>(List<Node> listNodes, T dataModel, Dictionary<string, object?> scope)
    {
        // Создаём StringBuilder для накопления результата
        StringBuilder resultBuilder = new();

        // Обходим все узлы в списке
        foreach (var res in listNodes)
        {
            // Диспетчеризация по типу узла
            switch (res)
            {
                // Текстовый узел — просто добавляем текст
                case TextNode textNode:
                    resultBuilder.Append(textNode.Text);
                    break;

                // Узел переменной — вычисляем значение операнда и приводим к строке
                case VariableNode varNode:
                    var operandValue = GetOperandValue(varNode.Value, dataModel, scope)
                        ?.ToString();
                    resultBuilder.Append(operandValue);
                    break;

                // Узел условия — делегируем в RenderIf
                case IfNode ifNode:
                    var resIf = RenderIf(ifNode, dataModel, scope);
                    resultBuilder.Append(resIf);
                    break;

                // Узел цикла — делегируем в RenderForeach
                case ForeachNode foreachNode:
                    var resForeach = RenderForeach(foreachNode, dataModel, scope);
                    resultBuilder.Append(resForeach);
                    break;
            }
        }

        // Возвращаем накопленную строку без крайних пробелов
        return resultBuilder.ToString().Trim();
    }

    /// <summary>
    /// Рендерит узел условия <c>$if</c>.
    /// </summary>
    /// <typeparam name="T">Тип модели данных.</typeparam>
    /// <param name="ifNode">Узел условия.</param>
    /// <param name="dataModel">Объект модели данных.</param>
    /// <param name="scope">Словарь переменных цикла.</param>
    /// <returns>Сгенерированная строка для выбранной ветки.</returns>
    /// <exception cref="ArgumentException">Если операнд не bool при отсутствии операции.</exception>
    private string RenderIf<T>(IfNode ifNode, T dataModel, Dictionary<string, object> scope)
    {
        // Вычисляем значение левого операнда
        var leftOperand = GetOperandValue(ifNode.Condition.Left, dataModel, scope);

        // Вычисляем значение правого операнда (может быть null)
        var rightOperand = GetOperandValue(ifNode.Condition.Right, dataModel, scope);

        // Получаем операцию сравнения (или null, если это просто проверка bool)
        var operation = ifNode.Condition.Operation;

        // Если операции нет — ожидаем, что левый операнд имеет тип bool
        if (operation is null)
        {
            // Проверяем, что левый операнд действительно bool
            if (leftOperand is bool leftOperandTrue)
            {
                // Если true — рендерим ветку then
                if (leftOperandTrue)
                    return RenderBlock(ifNode.ThenBranch, dataModel, scope);

                // Если false и есть else-ветка — рендерим её
                if (ifNode.ElseBranch != null)
                    return RenderBlock(ifNode.ElseBranch, dataModel, scope);

                // Иначе возвращаем пустую строку
                return "";
            }

            // Если операнд не bool — бросаем исключение
            throw new ArgumentException($"{leftOperand} не bool тип");
        }

        // Если операция есть — вычисляем булево выражение
        var isTrue = ValidateBooleanExpression(leftOperand, operation, rightOperand);

        // Если истина — рендерим ветку then
        if (isTrue)
            return RenderBlock(ifNode.ThenBranch, dataModel, scope);

        // Если есть else-ветка — рендерим её
        if (ifNode.ElseBranch != null)
            return RenderBlock(ifNode.ElseBranch, dataModel, scope);

        // Иначе возвращаем пустую строку
        return "";
    }

    /// <summary>
    /// Рендерит узел цикла <c>$foreach</c>.
    /// </summary>
    /// <typeparam name="T">Тип модели данных.</typeparam>
    /// <param name="foreachNode">Узел цикла.</param>
    /// <param name="dataModel">Объект модели данных.</param>
    /// <param name="scope">Словарь переменных цикла.</param>
    /// <returns>Сгенерированная строка для всех итераций.</returns>
    /// <exception cref="ArgumentException">Если операнд не является коллекцией.</exception>
    private string? RenderForeach<T>(ForeachNode foreachNode, T dataModel, Dictionary<string, object> scope)
    {
        // Получаем значение коллекции, которую нужно перебрать
        var collection = GetOperandValue(foreachNode.Collection, dataModel, scope);

        // Проверяем, что значение реализует IEnumerable
        if (collection is not IEnumerable enumerable)
            throw new ArgumentException("Объект переданный в foreach не является коллекцией");

        // Создаём StringBuilder для накопления результата
        StringBuilder resultBuilder = new();

        // Обходим каждый элемент коллекции
        foreach (var item in enumerable)
        {
            // Создаём новый scope, копируя внешний и добавляя переменную цикла
            var innerScope = new Dictionary<string, object>(scope)
            {
                [foreachNode.ItemName] = item
            };

            // Рендерим тело цикла с расширенным scope
            resultBuilder.Append(RenderBlock(foreachNode.Body, dataModel, innerScope));
        }

        // Возвращаем накопленную строку
        return resultBuilder.ToString();
    }

    /// <summary>
    /// Проверяет булево выражение сравнения двух операндов.
    /// </summary>
    /// <param name="leftOperand">Левый операнд.</param>
    /// <param name="operation">Операция сравнения.</param>
    /// <param name="rightOperand">Правый операнд.</param>
    /// <returns>Результат сравнения.</returns>
    /// <exception cref="ArgumentException">Если операция не поддерживается.</exception>
    /// <exception cref="InvalidOperationException">Если типы операндов несовместимы.</exception>
    private bool ValidateBooleanExpression(object leftOperand, string operation, object rightOperand)
    {
        // Проверяем, что оба операнда имеют числовой тип
        var isNumbers = NumericTypes.Any(t => t == leftOperand.GetType())
                        && NumericTypes.Any(t => t == rightOperand.GetType());

        // Ветвление по результату проверки
        switch (isNumbers)
        {
            // Оба операнда числовые — сравниваем через decimal
            case true:
            {
                // Приводим левый операнд к decimal
                var decimalLeftOperand = Convert.ToDecimal(leftOperand);
                // Приводим правый операнд к decimal
                var decimalRightOperand = Convert.ToDecimal(rightOperand);

                // Выполняем сравнение в зависимости от операции
                return operation switch
                {
                    "<=" => decimalLeftOperand <= decimalRightOperand,
                    ">=" => decimalLeftOperand >= decimalRightOperand,
                    "==" => decimalLeftOperand == decimalRightOperand,
                    "!=" => decimalLeftOperand != decimalRightOperand,
                    ">" => decimalLeftOperand > decimalRightOperand,
                    "<" => decimalLeftOperand < decimalRightOperand,
                    _ => throw new ArgumentException($"Операции {operation} не доступна")
                };
            }
            // Операнды не числовые — сравниваем как объекты
            case false:

                // Проверяем, что типы совпадают
                if (leftOperand.GetType() != rightOperand.GetType())
                    throw new InvalidOperationException(
                        $"Нельзя сравнить значения разных типов: {leftOperand.GetType().Name} и {rightOperand.GetType().Name}");

                // Поддерживаем только == и != для нечисловых типов
                return operation switch
                {
                    "==" => Equals(leftOperand, rightOperand),
                    "!=" => !Equals(leftOperand, rightOperand),
                    _ => throw new ArgumentException($"Операции {operation} не доступна")
                };
        }
    }

    /// <summary>
    /// Возвращает значение операнда (литерала или пути к свойству).
    /// </summary>
    /// <typeparam name="T">Тип модели данных.</typeparam>
    /// <param name="operand">Операнд.</param>
    /// <param name="dataModel">Объект модели данных.</param>
    /// <param name="scope">Словарь переменных цикла.</param>
    /// <returns>Значение операнда или null.</returns>
    /// <exception cref="ArgumentNullException">Если <paramref name="dataModel"/> равен null.</exception>
    /// <exception cref="ArgumentException">Если тип операнда не поддерживается.</exception>
    private object? GetOperandValue<T>(Operand? operand, T? dataModel, Dictionary<string, object> scope)
    {
        // Если операнд null — возвращаем null
        if (operand is null)
            return null;

        // Проверяем, что модель данных не null
        if (dataModel is null)
            throw new ArgumentNullException(nameof(dataModel), "Модель не должна быть null!");

        // Диспетчеризация по типу операнда
        return operand switch
        {
            // Путь к свойству — обрабатываем через PathOperandProcess
            PathOperand p => PathOperandProcess(p, dataModel, scope),
            // Литерал — возвращаем его значение
            LiteralOperand l => LiteralOperand(l),
            // Неизвестный тип — исключение
            _ => throw new ArgumentException("Операнд не обработан")
        };
    }

    /// <summary>
    /// Обрабатывает операнд-путь: проходит по сегментам и извлекает значение.
    /// </summary>
    /// <typeparam name="T">Тип модели данных.</typeparam>
    /// <param name="pathOperand">Операнд-путь.</param>
    /// <param name="dataModel">Объект модели данных.</param>
    /// <param name="scope">Словарь переменных цикла.</param>
    /// <returns>Значение свойства.</returns>
    /// <exception cref="ArgumentException">Если сегменты пусты или свойство не найдено.</exception>
    /// <exception cref="NullReferenceException">Если вложенное свойство не найдено.</exception>
    private object? PathOperandProcess<T>(PathOperand pathOperand, T? dataModel, Dictionary<string, object> scope)
    {
        // Получаем массив сегментов пути
        var segments = pathOperand.Segments;

        // Проверяем, что массив не пуст
        if (segments.Length == 0)
            throw new ArgumentException("Массив сегментов пуст!");

        // Определяем стартовый объект: переменная из scope или корневая модель
        var currentModel = scope.TryGetValue(segments[0], out var loopValue)
            ? loopValue
            : dataModel;

        // Если сегмент один — возвращаем сам объект (например, элемент цикла)
        if (segments.Length == 1)
            return currentModel;

        // Получаем PropertyInfo для первого свойства
        var finalProperty = currentModel?.GetType()
            .GetProperty(segments[1]);

        // Если свойство не найдено — исключение
        if (finalProperty == null)
            throw new ArgumentException(
                $"В предоставленной модели ({dataModel}) не найдено свойство {segments[1]}, которое указано в шаблоне!");

        // Если сегментов два — возвращаем значение свойства
        if (segments.Length == 2)
            return finalProperty.GetValue(currentModel)
                   ?? throw new ArgumentException($"Свойство {string.Join(".", segments)} = null");

        // Обходим оставшиеся сегменты пути
        for (var i = 1; i < segments.Length; i++)
        {
            // На последнем сегменте прекращаем обход
            if (i == segments.Length - 1)
                break;

            // Получаем значение текущего свойства
            var finalPropertyValue = finalProperty.GetValue(currentModel);

            // Если значение null — исключение
            if (finalPropertyValue == null)
                throw new ArgumentException($"Свойство {string.Join(".", segments)} = null");

            // Получаем тип значения
            var finalPropertyType = finalPropertyValue.GetType();

            // Получаем PropertyInfo для следующего сегмента
            finalProperty = finalPropertyType.GetProperty(segments[i + 1]);

            // Если следующее свойство не найдено — исключение
            if (finalProperty == null)
                throw new NullReferenceException("Вложенное свойство не найдено");

            // Переходим к следующему объекту
            currentModel = finalPropertyValue;
        }

        // Возвращаем значение последнего свойства
        return finalProperty.GetValue(currentModel)
               ?? throw new ArgumentException($"Свойство {string.Join(".", segments)} = null");
    }

    /// <summary>
    /// Извлекает значение из литерального операнда.
    /// </summary>
    /// <param name="literalOperand">Литеральный операнд.</param>
    /// <returns>Значение литерала.</returns>
    /// <exception cref="NullReferenceException">Если значение литерала равно null.</exception>
    private object LiteralOperand(LiteralOperand literalOperand)
    {
        // Возвращаем значение литерала или бросаем исключение, если оно null
        return literalOperand.Value
               ?? throw new NullReferenceException("");
    }

    /// <summary>
    /// Проверяет существование файла по указанному пути.
    /// </summary>
    /// <param name="filePath">Путь к файлу.</param>
    /// <exception cref="FileNotFoundException">Если файл не найден.</exception>
    private void CheckFileExists(string filePath)
    {
        // Если файл не существует - бросаем исключение
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Фала по пути: {filePath} не найдено");
    }
}
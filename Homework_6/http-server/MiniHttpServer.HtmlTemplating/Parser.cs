// Подключение абстракций: IParser, Node, Operand

using MiniHttpServer.HtmlTemplating.Abstractions;
using MiniHttpServer.HtmlTemplating.Expressions.Conditions;
using MiniHttpServer.HtmlTemplating.Expressions.Nodes;
using MiniHttpServer.HtmlTemplating.Expressions.Operands;
using MiniHttpServer.HtmlTemplating.Token;
// Подключение модели условия
// Подключение узлов AST
// Подключение операндов
// Подключение моделей токенов

// Пространство имён библиотеки
namespace MiniHttpServer.HtmlTemplating;

/// <summary>
/// Синтаксический анализатор: преобразует последовательность токенов
/// в дерево узлов (AST).
/// </summary>
public class Parser : IParser
{
    /// <summary>
    /// Текущая позиция в списке токенов.
    /// </summary>
    private int _pos;

    /// <summary>
    /// Список токенов, полученных от лексера.
    /// </summary>
    private List<TokenModel>? _tokens = null;

    /// <summary>
    /// Текущий токен (или null, если достигнут конец).
    /// </summary>
    private TokenModel? Current => _pos < _tokens?.Count ? _tokens[_pos] : null;

    /// <inheritdoc />
    public List<Node> Parse(List<TokenModel> tokens)
    {
        // Сохраняем список токенов, проверяя на null
        _tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));

        // Парсим блок верхнего уровня
        var nodes = ParseBlock();

        // Если остались необработанные токены - это ошибка
        return Current != null
            ?
            throw new Exception($"Неожиданная конструкция '{Current.Value}': нет соответствующего открывающего $if или $foreach")
            :
            nodes;
    }

    /// <summary>
    /// Разбирает блок узлов до закрывающего тега.
    /// </summary>
    /// <returns>Список узлов блока.</returns>
    /// <exception cref="Exception">При неожиданном токене.</exception>
    private List<Node> ParseBlock()
    {
        // Список для накопления узлов
        var nodes = new List<Node>();

        // Идём по токенам, пока не достигнем конца
        while (Current != null)
        {
            // Диспетчеризация по типу токена
            switch (Current.Type)
            {
                // Текстовый токен - создаём TextNode
                case TokenType.Text:
                    nodes.Add(new TextNode(Current.Value!));
                    _pos++;
                    break;

                // Токен переменной - создаём VariableNode с операндом
                case TokenType.Variable:
                    var variableNode = new VariableNode(ParseOperand(Current.Value!));
                    nodes.Add(variableNode);
                    _pos++;
                    break;

                // Токен $if - парсим узел условия
                case TokenType.If:
                    nodes.Add(ParseIf());
                    break;

                // Токен $foreach - парсим узел цикла
                case TokenType.Foreach:
                    nodes.Add(ParseForeach());
                    break;

                // Закрывающие токены - прекращаем разбор блока
                case TokenType.Else:
                case TokenType.EndIf:
                case TokenType.EndFor:
                    return nodes;

                // Неизвестный токен - исключение
                default:
                    throw new Exception($"Неожиданный токен {Current.Type} ('{Current.Value}')");
            }
        }

        // Возвращаем накопленные узлы
        return nodes;
    }

    /// <summary>
    /// Разбирает конструкцию <c>$if</c>.
    /// </summary>
    /// <returns>Узел <see cref="IfNode"/>.</returns>
    /// <exception cref="Exception">При некорректном синтаксисе условия.</exception>
    private IfNode ParseIf()
    {
        // Ожидаем токен $if
        Expect(TokenType.If);

        // Ожидаем открывающую скобку
        Expect(TokenType.OpenParenthesis);

        // Парсим левый операнд
        var left = ParseOperand(Expect(TokenType.Identifier).Value!);

        // Изначально операции и правого операнда нет
        string? operation = null;
        Operand? right = null;

        // Если следующий токен - операция, парсим её и правый операнд
        if (Current?.Type == TokenType.Operation)
        {
            operation = Expect(TokenType.Operation).Value;
            right = ParseOperand(Expect(TokenType.Identifier).Value!);
        }

        // Ожидаем закрывающую скобку
        Expect(TokenType.CloseParenthesis);

        // Парсим ветку then
        var thenBranch = ParseBlock();

        // Изначально ветки else нет
        List<Node>? elseBranch = null;

        // Если следующий токен - $else, парсим ветку else
        if (Current?.Type == TokenType.Else)
        {
            _pos++;
            elseBranch = ParseBlock();
        }

        // Ожидаем $endif
        if (Current?.Type != TokenType.EndIf)
            throw new Exception("$if не закрыт: ожидался $endif");

        // Пропускаем токен $endif
        _pos++;

        // Возвращаем узел условия
        return new IfNode(
            new Condition(left, operation, right),
            thenBranch,
            elseBranch);
    }

    /// <summary>
    /// Разбирает конструкцию <c>$foreach</c>.
    /// </summary>
    /// <returns>Узел <see cref="ForeachNode"/>.</returns>
    /// <exception cref="Exception">При некорректном синтаксисе цикла.</exception>
    private ForeachNode ParseForeach()
    {
        // Ожидаем токен $foreach
        Expect(TokenType.Foreach);

        // Ожидаем открывающую скобку
        Expect(TokenType.OpenParenthesis);

        // Парсим имя переменной цикла
        var itemName = Expect(TokenType.Identifier).Value!;

        // Парсим текст коллекции
        var collectionText = Expect(TokenType.Identifier).Value!;

        // Ожидаем закрывающую скобку
        Expect(TokenType.CloseParenthesis);

        // Проверяем, что коллекция - это путь к свойству, а не литерал
        if (ParseOperand(collectionText) is not PathOperand collection)
            throw new Exception($"В $foreach нельзя перебирать константу '{collectionText}'");

        // Парсим тело цикла
        var body = ParseBlock();

        // Ожидаем $endfor
        if (Current?.Type != TokenType.EndFor)
            throw new Exception("$foreach не закрыт: ожидался $endfor");

        // Пропускаем токен $endfor
        _pos++;

        // Возвращаем узел цикла
        return new ForeachNode(itemName, collection, body);
    }

    /// <summary>
    /// Определяет, что перед нами: константа или путь к свойству.
    /// </summary>
    /// <param name="text">Текст операнда.</param>
    /// <returns>Операнд (<see cref="LiteralOperand"/> или <see cref="PathOperand"/>).</returns>
    /// <exception cref="Exception">Если выражение некорректно.</exception>
    private static Operand ParseOperand(string text)
    {
        // Обрезаем пробелы
        text = text.Trim();

        // Обрабатываем булевы литералы
        switch (text)
        {
            case "true":
                return new LiteralOperand(true);
            case "false":
                return new LiteralOperand(false);
        }

        // Проверка на то что это строка (в кавычках)
        if (text.Length >= 2 && text[0] == '"' && text[^1] == '"')
            return new LiteralOperand(text.Substring(1, text.Length - 2));

        // Проверка на то что это число
        if (int.TryParse(text, out var number))
            return new LiteralOperand(number);

        // Разбиваем путь на сегменты по точке
        var segments = text.Split('.');

        // Проверяем каждый сегмент на корректность
        foreach (var segment in segments)
        {
            if (!IsValidIdentifier(segment))
                throw new Exception($"Некорректное выражение '{text}'");
        }

        // Возвращаем операнд-путь
        return new PathOperand(segments);
    }

    /// <summary>
    /// Проверяет, является ли строка корректным идентификатором.
    /// </summary>
    /// <param name="s">Проверяемая строка.</param>
    /// <returns>true, если идентификатор корректен.</returns>
    private static bool IsValidIdentifier(string s)
    {
        // Пустая строка или строка, начинающаяся с цифры, - не идентификатор
        if (s.Length == 0 || char.IsDigit(s[0]))
            return false;

        // Все символы должны быть буквами, цифрами или '_'
        return s.All(c => char.IsLetterOrDigit(c) || c == '_');
    }

    /// <summary>
    /// Проверяет тип текущего токена, возвращает его и сдвигает позицию вперёд.
    /// </summary>
    /// <param name="type">Ожидаемый тип токена.</param>
    /// <returns>Текущий токен.</returns>
    /// <exception cref="Exception">Если токен отсутствует или имеет другой тип.</exception>
    private TokenModel Expect(TokenType type)
    {
        // Если достигнут конец - исключение
        if (Current == null)
            throw new Exception($"Неожиданный конец шаблона, ожидался {type}");

        // Если тип не совпадает - исключение
        if (Current.Type != type)
            throw new Exception($"Ожидался {type}, но найден {Current.Type} ('{Current.Value}')");

        // Возвращаем текущий токен и сдвигаем позицию
        return _tokens[_pos++];
    }
}
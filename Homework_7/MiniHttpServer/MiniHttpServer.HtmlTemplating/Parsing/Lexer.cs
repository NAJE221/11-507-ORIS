// Подключение абстракций: ILexer

using MiniHttpServer.HtmlTemplating.Abstractions;
using MiniHttpServer.HtmlTemplating.Token;

// Подключение моделей токенов: TokenModel, TokenType

// Пространство имён библиотеки
namespace MiniHttpServer.HtmlTemplating.Parsing;

/// <summary>
/// Лексический анализатор шаблонов: разбивает строку шаблона
/// на последовательность токенов.
/// </summary>
public class Lexer : ILexer
{
    /// <summary>
    /// Поддерживаемые операторы сравнения.
    /// </summary>
    private static string[] _operators = ["==", "!=", ">=", "<=", ">", "<"];

    /// <summary>
    /// Поддерживаемые ключевые слова конструкций.
    /// </summary>
    private static string[] _keywords = ["if", "else", "endif", "foreach", "endfor"];

    /// <inheritdoc />
    public List<TokenModel> Tokenize(string template)
    {
        // Список для накопления токенов
        var tokens = new List<TokenModel>();

        // Текущая позиция в строке шаблона
        var pos = 0;

        // Идём по строке, пока не достигнем конца
        while (pos < template.Length)
        {
            // Ищем ближайший символ '$' начиная с текущей позиции
            var dollar = template.IndexOf('$', pos);

            // Если '$' не найден - оставшийся текст добавляем как Text-токен и выходим
            if (dollar == -1)
            {
                tokens.Add(new TokenModel(TokenType.Text, template.Substring(pos)));
                break;
            }

            // Если '$' - последний символ строки, конструкция некорректна
            if (dollar == template.Length - 1)
                throw new Exception($"На позиции {dollar} некорректная конструкция $");

            // Если между pos и dollar есть текст - добавляем его как Text-токен
            if (dollar > pos)
                tokens.Add(new TokenModel(TokenType.Text, template.Substring(pos, dollar - pos)));

            // Если после '$' идёт '{' - это переменная ${...}
            if (template[dollar + 1] == '{')
            {
                // Обрабатываем переменную и получаем новую позицию
                pos = ProcessVariable(template, dollar, tokens);
                continue;
            }

            // Иначе определяем ключевое слово после '$'
            var keyword = GetKeyword(template, dollar);

            // Диспетчеризация по ключевому слову
            switch (keyword)
            {
                // Обработка $if(...)
                case "if":
                    pos = ProcessIf(template, dollar, tokens);
                    break;
                // Обработка $foreach(...)
                case "foreach":
                    pos = ProcessForeach(template, dollar, tokens);
                    break;
                // Обработка $else
                case "else":
                    tokens.Add(new TokenModel(TokenType.Else, "else"));
                    pos = dollar + 1 + keyword.Length;
                    break;
                // Обработка $endif
                case "endif":
                    tokens.Add(new TokenModel(TokenType.EndIf, "endif"));
                    pos = dollar + 1 + keyword.Length;
                    break;
                // Обработка $endfor
                case "endfor":
                    tokens.Add(new TokenModel(TokenType.EndFor, "endfor"));
                    pos = dollar + 1 + keyword.Length;
                    break;
                // Неизвестная конструкция - исключение
                default:
                    throw new Exception($"Неизвестная конструкция на позиции {dollar}");
            }
        }

        // Возвращаем накопленные токены
        return tokens;
    }

    /// <summary>
    /// Обрабатывает переменную <c>${...}</c>.
    /// </summary>
    /// <param name="template">Строка шаблона.</param>
    /// <param name="dollar">Позиция символа '$'.</param>
    /// <param name="tokens">Список токенов для пополнения.</param>
    /// <returns>Позиция после закрывающей '}'.</returns>
    /// <exception cref="Exception">Если не найдена '}' или переменная пуста.</exception>
    private static int ProcessVariable(string template, int dollar, List<TokenModel> tokens)
    {
        // Ищем закрывающую '}' начиная после "${"
        var closingBrace = template.IndexOf('}', dollar + 2);

        // Если '}' не найдена - исключение
        if (closingBrace == -1)
            throw new Exception($"Для {{ на позиции {dollar + 1} не найдена }}");

        // Извлекаем имя переменной и обрезаем пробелы
        var variable = template.Substring(dollar + 2, closingBrace - dollar - 2).Trim();

        // Если имя пустое - исключение
        if (variable.Length == 0)
            throw new Exception($"Пустая переменная на позиции {dollar}");

        // Добавляем токен переменной
        tokens.Add(new TokenModel(TokenType.Variable, variable));

        // Возвращаем позицию после '}'
        return closingBrace + 1;
    }

    /// <summary>
    /// Обрабатывает конструкцию <c>$if(...)</c>.
    /// </summary>
    /// <param name="template">Строка шаблона.</param>
    /// <param name="dollar">Позиция символа '$'.</param>
    /// <param name="tokens">Список токенов для пополнения.</param>
    /// <returns>Позиция после закрывающей ')'.</returns>
    /// <exception cref="Exception">При некорректном синтаксисе условия.</exception>
    private static int ProcessIf(string template, int dollar, List<TokenModel> tokens)
    {
        // Позиция, где должна быть '(' после "if"
        var openParen = dollar + 1 + "if".Length;

        // Проверяем, что после $if идёт '('
        if (openParen >= template.Length || template[openParen] != '(')
            throw new Exception($"После $if на позиции {dollar} ожидается '('");

        // Ищем закрывающую ')'
        var closeParen = template.IndexOf(')', openParen + 1);

        // Если ')' не найдена - исключение
        if (closeParen == -1)
            throw new Exception($"Не найдена ')' для '(' на позиции {openParen}");

        // Извлекаем выражение условия
        var expression = template.Substring(openParen + 1, closeParen - openParen - 1).Trim();

        // Если выражение пустое - исключение
        if (expression.Length == 0)
            throw new Exception($"Пустое условие в $if на позиции {dollar}");

        // Добавляем токен начала $if
        tokens.Add(new TokenModel(TokenType.If, "if"));

        // Добавляем токен открывающей скобки
        tokens.Add(new TokenModel(TokenType.OpenParenthesis, "("));

        // Ищем первую подходящую операцию в выражении
        var operation = _operators.FirstOrDefault(o => expression.Contains(o, StringComparison.Ordinal));

        // Если операции нет - это просто проверка bool-значения
        if (operation == null)
        {
            tokens.Add(new TokenModel(TokenType.Identifier, expression));
        }
        else
        {
            // Находим позицию операции в выражении
            var idx = expression.IndexOf(operation, StringComparison.Ordinal);

            // Извлекаем левый операнд
            var left = expression.Substring(0, idx).Trim();

            // Извлекаем правый операнд
            var right = expression.Substring(idx + operation.Length).Trim();

            // Проверяем, что левый операнд не пуст
            if (left.Length == 0)
                throw new Exception($"Слева от '{operation}' пусто (конструкция на позиции {dollar})");

            // Проверяем, что правый операнд не пуст
            if (right.Length == 0)
                throw new Exception($"Справа от '{operation}' пусто (конструкция на позиции {dollar})");

            // Добавляем токены: левый операнд, операция, правый операнд
            tokens.Add(new TokenModel(TokenType.Identifier, left));
            tokens.Add(new TokenModel(TokenType.Operation, operation));
            tokens.Add(new TokenModel(TokenType.Identifier, right));
        }

        // Добавляем токен закрывающей скобки
        tokens.Add(new TokenModel(TokenType.CloseParenthesis, ")"));

        // Возвращаем позицию после ')'
        return closeParen + 1;
    }

    /// <summary>
    /// Обрабатывает конструкцию <c>$foreach(...)</c>.
    /// </summary>
    /// <param name="template">Строка шаблона.</param>
    /// <param name="dollar">Позиция символа '$'.</param>
    /// <param name="tokens">Список токенов для пополнения.</param>
    /// <returns>Позиция после закрывающей ')'.</returns>
    /// <exception cref="Exception">При некорректном синтаксисе цикла.</exception>
    private static int ProcessForeach(string template, int dollar, List<TokenModel> tokens)
    {
        // Позиция, где должна быть '(' после "foreach"
        var openParen = dollar + 1 + "foreach".Length;

        // Проверяем, что после $foreach идёт '('
        if (openParen >= template.Length || template[openParen] != '(')
            throw new Exception($"После $foreach на позиции {dollar} ожидается '('");

        // Ищем закрывающую ')'
        var closeParen = template.IndexOf(')', openParen + 1);

        // Если ')' не найдена - исключение
        if (closeParen == -1)
            throw new Exception($"Не найдена ')' для '(' на позиции {openParen}");

        // Извлекаем выражение внутри скобок
        var expression = template.Substring(openParen + 1, closeParen - openParen - 1).Trim();

        // Разбиваем на две части по " in "
        var parts = expression.Split(" in ", 2, StringSplitOptions.TrimEntries);

        // Проверяем, что получилось две непустые части
        if (parts.Length != 2 || parts[0].Length == 0 || parts[1].Length == 0)
            throw new Exception($"Некорректный $foreach на позиции {dollar}, ожидается (var x in collection)");

        // Извлекаем имя переменной цикла (убирая префикс "var ")
        var itemName = parts[0].StartsWith("var ") ? parts[0].Substring(4).Trim() : parts[0];

        // Добавляем токены: начало foreach, '(', имя переменной, коллекция, ')'
        tokens.Add(new TokenModel(TokenType.Foreach, "foreach"));
        tokens.Add(new TokenModel(TokenType.OpenParenthesis, "("));
        tokens.Add(new TokenModel(TokenType.Identifier, itemName));
        tokens.Add(new TokenModel(TokenType.Identifier, parts[1]));
        tokens.Add(new TokenModel(TokenType.CloseParenthesis, ")"));

        // Возвращаем позицию после ')'
        return closeParen + 1;
    }

    /// <summary>
    /// Определяет ключевое слово после символа '$'.
    /// </summary>
    /// <param name="text">Строка шаблона.</param>
    /// <param name="dollarPosition">Позиция символа '$'.</param>
    /// <returns>Ключевое слово или null, если не найдено.</returns>
    private static string? GetKeyword(string text, int dollarPosition)
    {
        // Позиция сразу после '$'
        var start = dollarPosition + 1;

        // Срез строки начиная с позиции после '$'
        var rest = text.AsSpan(start);

        // Перебираем все ключевые слова
        foreach (var keyword in _keywords)
        {
            // Если срез не начинается с ключевого слова - пропускаем
            if (!rest.StartsWith(keyword, StringComparison.Ordinal))
                continue;

            // Позиция сразу после ключевого слова
            var next = start + keyword.Length;

            // Если следующий символ - буква или цифра, это не отдельное слово
            if (next < text.Length && char.IsLetterOrDigit(text[next]))
                continue;

            // Возвращаем найденное ключевое слово
            return keyword;
        }

        // Ничего не нашли
        return null;
    }
}
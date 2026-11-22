namespace MiniHttpServer.HtmlTemplating.Token;

/// <summary>
/// Типы токенов, распознаваемых лексером.
/// </summary>
public enum TokenType
{
    /// <summary>Статический текст.</summary>
    Text,
    /// <summary>Переменная ${...}.</summary>
    Variable,
    /// <summary>Начало условия $if.</summary>
    If,
    /// <summary>Ветка $else.</summary>
    Else,
    /// <summary>Конец условия $endif.</summary>
    EndIf,
    /// <summary>Начало цикла $foreach.</summary>
    Foreach,
    /// <summary>Конец цикла $endfor.</summary>
    EndFor,
    /// <summary>Операция сравнения.</summary>
    Operation,
    /// <summary>Идентификатор. (User.Age в if и т.п)</summary>
    Identifier,
    /// <summary>Открывающая скобка.</summary>
    OpenParenthesis,
    /// <summary>Закрывающая скобка.</summary>
    CloseParenthesis
}
namespace MiniHttpServer.HtmlTemplating.Token;

/// <summary>
/// Модель токена: тип и значение.
/// </summary>
public class TokenModel(TokenType type, string value)
{
    /// <summary>Тип токена.</summary>
    public TokenType Type { get; } = type;

    /// <summary>Текстовое значение токена.</summary>
    public string? Value { get; } = value;
}
using MiniHttpServer.HtmlTemplating.Token;

namespace MiniHttpServer.HtmlTemplating.Abstractions;

/// <summary>
/// Определяет контракт лексического анализатора, разбивающего
/// строку шаблона на последовательность токенов.
/// </summary>
public interface ILexer
{
    /// <summary>
    /// Разбивает строку шаблона на список токенов.
    /// </summary>
    /// <param name="template">Исходная строка шаблона.</param>
    /// <returns>Список токенов.</returns>
    public List<TokenModel> Tokenize(string template);
}
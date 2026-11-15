using MiniHttpServer.HtmlTemplating.Token;

namespace MiniHttpServer.HtmlTemplating.Abstractions;

/// <summary>
/// Определяет контракт синтаксического анализатора, преобразующего
/// последовательность токенов в дерево узлов (AST).
/// </summary>
public interface IParser
{
    /// <summary>
    /// Преобразует список токенов в список узлов дерева разбора.
    /// </summary>
    /// <param name="tokens">Список токенов.</param>
    /// <returns>Список узлов AST.</returns>
    public List<Node> Parse(List<TokenModel> tokens);
}
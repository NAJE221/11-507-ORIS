using MiniHttpServer.HtmlTemplating.Abstractions;

namespace MiniHttpServer.HtmlTemplating.Expressions.Nodes;

/// <summary>
/// Узел статического текста шаблона.
/// </summary>
public class TextNode(string text) : Node
{
    /// <summary>Текстовое содержимое.</summary>
    public string Text { get; } = text;
}
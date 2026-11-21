using MiniHttpServer.HtmlTemplating.Abstractions;
using MiniHttpServer.HtmlTemplating.Expressions.Conditions;

namespace MiniHttpServer.HtmlTemplating.Expressions.Nodes;

/// <summary>
/// Узел условия $if с ветками then и (опционально) else.
/// </summary>
public class IfNode(Condition condition, List<Node> thenBranch, List<Node>? elseBranch) : Node
{
    /// <summary>Условие ветвления.</summary>
    public Condition Condition { get; } = condition;

    /// <summary>Ветка «тогда».</summary>
    public List<Node> ThenBranch { get; } = thenBranch;

    /// <summary>Ветка «иначе» (может отсутствовать).</summary>
    public List<Node>? ElseBranch { get; } = elseBranch;
}



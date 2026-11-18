using MiniHttpServer.HtmlTemplating.Abstractions;

namespace MiniHttpServer.HtmlTemplating.Expressions.Nodes;

/// <summary>
/// Узел подстановки переменной (${...}).
/// </summary>
public class VariableNode(Operand value) : Node
{
    /// <summary>Операнд, значение которого подставляется.</summary>
    public Operand Value { get; } = value;
}
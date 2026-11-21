using MiniHttpServer.HtmlTemplating.Abstractions;
using MiniHttpServer.HtmlTemplating.Expressions.Operands;

namespace MiniHttpServer.HtmlTemplating.Expressions.Nodes;

/// <summary>
/// Узел цикла $foreach.
/// </summary>
public class ForeachNode(string itemName, PathOperand collection, List<Node> body) : Node
{
    /// <summary>Имя переменной-элемента внутри цикла.</summary>
    public string ItemName { get; } = itemName;

    /// <summary>Путь к перебираемой коллекции.</summary>
    public PathOperand Collection { get; } = collection;

    /// <summary>Тело цикла.</summary>
    public List<Node> Body { get; } = body;
}
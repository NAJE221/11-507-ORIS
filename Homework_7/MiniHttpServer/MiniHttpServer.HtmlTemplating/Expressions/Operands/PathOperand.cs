using MiniHttpServer.HtmlTemplating.Abstractions;

namespace MiniHttpServer.HtmlTemplating.Expressions.Operands;

/// <summary>
/// Операнд-путь к свойству, разбитый на сегменты (например, "user.address.city").
/// </summary>
public class PathOperand(string[] segments) : Operand
{
    /// <summary>Сегменты пути.</summary>
    public string[] Segments { get; } = segments;
}
using MiniHttpServer.HtmlTemplating.Abstractions;

namespace MiniHttpServer.HtmlTemplating.Expressions.Operands;

/// <summary>
/// Операнд-литерал (константа: число, строка, true/false).
/// </summary>
public class LiteralOperand(object value) : Operand
{
    /// <summary>Значение литерала.</summary>
    public object? Value { get; } = value;
}
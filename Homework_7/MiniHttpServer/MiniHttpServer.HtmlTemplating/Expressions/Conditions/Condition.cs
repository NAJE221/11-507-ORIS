using MiniHttpServer.HtmlTemplating.Abstractions;

namespace MiniHttpServer.HtmlTemplating.Expressions.Conditions;

/// <summary>
/// Модель условия для конструкции $if.
/// </summary>
public class Condition(Operand left, string? operation, Operand? right)
{
    /// <summary>Левый операнд условия.</summary>
    public Operand Left { get; } = left;

    /// <summary>Операция сравнения ("==", "!=", ">=" и т.п.) или null.</summary>
    public string? Operation { get; } = operation;

    /// <summary>Правый операнд условия (может отсутствовать).</summary>
    public Operand? Right { get; } = right;
}
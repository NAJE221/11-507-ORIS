namespace MiniHttpServer.Framework.Framework.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class HttpControllerAttribute(string route) : Attribute
{
    public string Route { get; } = route;
}

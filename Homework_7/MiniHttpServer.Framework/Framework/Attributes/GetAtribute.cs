namespace MiniHttpServer.Framework.Framework.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class GetAttribute(string route) : HttpMethodAttribute
{
    public string Route { get; }  = route;
}
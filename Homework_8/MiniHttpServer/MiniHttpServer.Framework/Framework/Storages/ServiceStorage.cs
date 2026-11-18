namespace MiniHttpServer.Framework.Framework.Storages;

public class ServiceStorage
{
    private readonly List<object> _services = new();

    public void Add<T>(T service) where T : class
    {
        _services.Add(service);
    }

    public object? Get(Type type)
        => _services.FirstOrDefault(type.IsInstanceOfType);
}
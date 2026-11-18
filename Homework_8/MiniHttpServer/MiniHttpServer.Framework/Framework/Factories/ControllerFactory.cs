using System.Net;
using MiniHttpServer.Framework.Framework.Storages;

namespace MiniHttpServer.Framework.Framework.Factories;

public static class ControllerFactory
{
    public static object Create(
        Type controllerType,
        HttpListenerContext context,
        ServiceStorage services)
    {
        var constructor = controllerType
            .GetConstructors()
            .Single();

        var requestScoped = new Dictionary<Type, object>
        {
            [typeof(HttpListenerContext)]  = context,
            [typeof(HttpListenerRequest)]  = context.Request,
            [typeof(HttpListenerResponse)] = context.Response
        };

        var parameters = constructor.GetParameters();
        var arguments = new object[parameters.Length];

        for (var i = 0; i < parameters.Length; i++)
        {
            var type = parameters[i].ParameterType;

            arguments[i] = requestScoped.GetValueOrDefault(type)
                           ?? services.Get(type)
                           ?? throw new InvalidOperationException(
                               $"Сервис {type.Name} не зарегистрирован " +
                               $"(нужен для {controllerType.Name})");
        }

        return constructor.Invoke(arguments);
    }
}
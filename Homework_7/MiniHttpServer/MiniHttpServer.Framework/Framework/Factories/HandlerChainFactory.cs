using MiniHttpServer.Framework.Framework.Abstractions;

namespace MiniHttpServer.Framework.Framework.Factories;

public class HandlerChainFactory()
{
    public static Handler AssembleChain(params Handler[] handlers)
    {
        for (var i = 0; i < handlers.Length; i++)
        {
            handlers[i].Successor = i == handlers.Length - 1
                ? null
                : handlers[i + 1];
        }
        return handlers[0];
    }
}
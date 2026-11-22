using System.Net;

namespace MiniHttpServer.Framework.Framework.Abstractions;

public abstract class Handler
{
    public Handler? Successor { get; set; }
    public abstract Task HandlerRequest(HttpListenerContext request);
}

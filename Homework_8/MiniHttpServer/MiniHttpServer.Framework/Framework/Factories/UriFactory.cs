using MiniHttpServer.Framework.Framework.Options;

namespace MiniHttpServer.Framework.Framework.Factories;

public static class UriFactory
{
    public static UriBuilder CreateUri(ServerOptions serverOptions)
    {
        UriBuilder uriBuilder = new()
        {
            Scheme = "http",
            Host = serverOptions.Host,
            Port = serverOptions.Port,
        };

        return uriBuilder;
    }
}
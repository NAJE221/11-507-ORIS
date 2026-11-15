using http_server.Framework.DTOs;

namespace http_server.Framework.Factories;

public static class UriFactory
{
    private static UriBuilder CreateUri(Server server)
    {
        UriBuilder uriBuilder = new()
        {
            Scheme = "http",
            Host = server.Host,
            Port = server.Port
        };

        return uriBuilder;
    }

    public static Uri GetUri(this Server server)
    {
        return CreateUri(server).Uri;
    }
}
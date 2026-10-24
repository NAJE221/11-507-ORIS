using System.Net;

namespace http_server.Framework.Http;

public class ResourceValidator
{
    public static void SendNotFoundPage(HttpListenerResponse response, ref string filePath)
    {
        if (!File.Exists(filePath))
        {
            response.StatusCode = 404;
            filePath = Path.Combine(Directory.GetCurrentDirectory(), "static", "404.html");
        }
    }
}
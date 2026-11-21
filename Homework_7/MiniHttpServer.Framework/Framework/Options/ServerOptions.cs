using System.Text.Json.Serialization;

namespace MiniHttpServer.Framework.Framework.Options;


public class ServerOptions
{
    public string Host { get; set; } = "127.0.0.1";
    public int Port { get; set; } = 8080;
    
    public string Path { get; set; } = "content/";
}



using System.Text.Json.Serialization;

public class Setting
{
    [JsonPropertyName("Server")]
    public Server server {get; set;} =  new();
}
    

public class Server
{
    public string host { get; set; } = "127.0.0.1";
    public string port { get; set; } = "8080";
    public string path { get; set; } = "content/";
}


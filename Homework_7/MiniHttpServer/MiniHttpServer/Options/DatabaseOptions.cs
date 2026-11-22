using System.Text.Json.Serialization;

namespace MiniHttpServer.Options;

public class DatabaseOptions
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 5432;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string DataBase { get; set; } = string.Empty;
    
    [JsonIgnore]
    public string ConnectionString 
        => $"Host={Host};Port={Port};Username={Username};Password={Password};Database={DataBase}";
}
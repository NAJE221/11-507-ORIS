using System.Text.Json;
using http_server.Framework.DTOs;

namespace http_server.Framework.Services;

public static class DataDeserializerService
{
    public static Setting Prefix { get; } = JsonSerialize();

    private static Setting JsonSerialize()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "server.json");
        try
        {
            
            return JsonSerializer.Deserialize<Setting>(File.ReadAllText(filePath)) 
                      ?? new Setting();
        }
        catch (FileNotFoundException)
        {
            var jsonText = JsonSerializer.Serialize(new Setting());
            File.WriteAllText(filePath, jsonText);
            return new Setting();
        }
        catch (JsonException)   
        {
            Console.WriteLine($"Файл имеет ошибки. Перепроверьте файл, и попробуй снова");
            return new Setting();
        }
    }
    
}
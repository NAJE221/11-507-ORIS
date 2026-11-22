using System.Text.Json;

namespace MiniHttpServer.Services;

public static class DataDeserializerService
{
    public static async Task<T> DeserializeAsync<T>(
        string filePath,
        string sectionName = "DatabaseServerOptions",
        CancellationToken cancellationToken = default)
    {
        string json;
        try
        {
            json = await File.ReadAllTextAsync(filePath, cancellationToken);
        }
        catch (Exception ex) when (ex is FileNotFoundException or DirectoryNotFoundException)
        {
            throw new FileNotFoundException($"Файл не найден: {filePath}", filePath, ex);
        }

        try
        {
            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty(sectionName, out var section))
                throw new InvalidOperationException(
                    $"В файле не найдена секция '{sectionName}'");

            return section.Deserialize<T>()
                   ?? throw new InvalidOperationException(
                       $"Десериализация секции '{sectionName}' вернула null");
        }
        catch (JsonException ex)
        {
            throw new JsonException($"Файл '{filePath}' содержит некорректный json", ex);
        }
    }
}
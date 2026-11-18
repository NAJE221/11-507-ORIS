using MiniHttpServer.Options;

namespace MiniHttpServer.Services;

public class LoaderDbOptionsService
{
    public static async Task<string> LoadOptions(string filePath, string schema)
    {
        var dbOptions = await DataDeserializerService.DeserializeAsync<DbOptions>
            (filePath, schema);
        
        return dbOptions.ConnectionString;
    }
}
using MiniHttpServer.Framework.Framework.Options;

namespace MiniHttpServer.Framework.Framework.Services;

public class ConfigurationLoaderService
{
    public static Task<ServerOptions> Load(string filePath , string sectionName)
    {
        return JsonConfigurationLoader.DeserializeAsync<ServerOptions>(filePath, sectionName);
    }
}
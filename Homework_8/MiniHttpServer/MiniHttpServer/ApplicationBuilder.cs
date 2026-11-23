using MiniHttpServer.Framework.Framework.Builder;
using MiniHttpServer.Framework.Framework.Storages;
using MiniHttpServer.ORM;
using MiniHttpServer.ORM.Abstractions;
using MiniHttpServer.ORM.Driver;
using MiniHttpServer.Services;

namespace MiniHttpServer;

public class ApplicationBuilder
{
    private async Task<IOrmContext> CreateOrmContext()
    {
        var connectionString = await LoaderDbOptionsService.LoadOptions
            // Настройки DB
            ("server.json" , "DatabaseServerOptions");

        // Создание драйвера (в данном случае NpgSql)
        IDbDriver driver = new NpgsqlDbDriver(connectionString);
        
        return new OrmContext(driver);
    }
    
    private async Task<ServiceStorage> CreateServiceStorageStorage()
    {
        var context = await CreateOrmContext();
        var storage = new ServiceStorage();
        
        storage.Add(new UserServices(context));
        
        return storage;
    }
    
    private async Task<FrameworkBuilder> CreateServerBuilder()
    {
        var storage = await CreateServiceStorageStorage();
        
        return await FrameworkBuilder.CreateBuilder
            // Настройки сервера
            ("server.json" , "Server" , storage);
    }
    

    public async Task Build()
    {
        var builder = await CreateServerBuilder();
        
        await builder.Server.RunAsync();
    }
}
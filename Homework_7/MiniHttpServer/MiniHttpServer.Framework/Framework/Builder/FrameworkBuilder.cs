using System.Net;
using MiniHttpServer.Framework.Framework.Abstractions;
using MiniHttpServer.Framework.Framework.Factories;
using MiniHttpServer.Framework.Framework.Handlers;
using MiniHttpServer.Framework.Framework.Options;
using MiniHttpServer.Framework.Framework.Routing;
using MiniHttpServer.Framework.Framework.Services;

namespace MiniHttpServer.Framework.Framework.Builder;

public class FrameworkBuilder
{
    public readonly HttpServer Server;

    private FrameworkBuilder(ServerOptions serverOptions)
    {
        RouteRegistry.Register();
        
        
        var listener = new HttpListener();
        
        IResourceService resourceService = new ResourceService();

        Handler controllerHandler = new ControllerHandler(resourceService);
        Handler fileHandler = new StaticFileHandler(resourceService);

        var chain = HandlerChainFactory.AssembleChain(
            fileHandler,
            controllerHandler);

        var prefix = UriFactory.CreateUri(serverOptions).Uri;
        listener.Prefixes.Add(prefix.ToString());
        
        
        Server = new HttpServer(listener , chain);
    }

    public static async Task<FrameworkBuilder> CreateBuilder(string filePath, string sectionName)
    {
        var options = await ConfigurationLoaderService.Load(filePath, sectionName);

        return new FrameworkBuilder(options);
    }
}
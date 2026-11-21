using System.Net;
using MiniHttpServer.Framework.Framework.Abstractions;
using static MiniHttpServer.Framework.Framework.Services.FileService;


namespace MiniHttpServer.Framework.Framework.Handlers;

internal class StaticFileHandler(IResourceService resourceService) : Handler
{
    
    public override async Task HandlerRequest(HttpListenerContext context)
    {
        var request = context.Request;
        Console.WriteLine($"\nЗапрос пришел : {request.Url?.LocalPath}");
        
        var userPath = request.Url?.LocalPath.Trim('/') ?? string.Empty;
        
        var isFile   = !string.IsNullOrEmpty(userPath) && Path.HasExtension(userPath);
        
        if (isFile)
        {
            var staticRoot = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "static"));
            var filePath         = Path.GetFullPath(Path.Combine(staticRoot, userPath));

            var response = context.Response;
            
            Console.WriteLine(filePath);
            
            if (!await resourceService.ResolveFile(filePath, context))
                return;
            
            await FilePush(filePath, response);
            
            Console.WriteLine("Запрос обработан");
            context.Response.Close();
        }

        else if (Successor != null)
            await Successor.HandlerRequest(context);
    }   
}
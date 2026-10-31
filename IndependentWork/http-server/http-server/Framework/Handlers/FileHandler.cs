using System.Net;
using http_server.Framework.Abstractions.Base;
using http_server.Framework.Services;
using static http_server.Framework.Services.FileService;


namespace http_server.Framework.Handlers;

internal class FileHandler(ResourceService resourceService) : Handler
{
    
    public override async Task HandlerRequest(HttpListenerContext context)
    {
        var request = context.Request;
        
        Console.WriteLine($"\nЗапрос пришел : {request.Url?.LocalPath}");
        
        var userPath = request.Url?.LocalPath.Trim('/');

        var isFile = userPath!.Contains('.');
        
        if (isFile)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "static", userPath);

            var response = context.Response;
            
            Console.WriteLine(filePath);
            
            
            var result = await resourceService.GetPageNotFound(filePath, context);
            if (!result) return;
            
            await FilePush(filePath, response);
            
            Console.WriteLine("Запрос обработан");
            context.Response.Close();
        }

        else if (Successor != null)
        {
            await Successor.HandlerRequest(context);
        }
    }   
}
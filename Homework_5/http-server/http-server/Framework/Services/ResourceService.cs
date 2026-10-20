using System.Net;
using static http_server.Framework.Services.FileService;
using http_server.Framework.Abstractions.Interfaces;

namespace http_server.Framework.Services;

public class ResourceService : IResourceService
{
    public async Task<bool> GetPageNotFound(string filePath, HttpListenerContext context)
    {
        if (File.Exists(filePath)) return true;
        await SendErrorPage("404", context);
        return false;
    }
    
    public async Task<bool> CheckNull(HttpListenerContext context, string codeError, params object?[] objects)
    {
        foreach (var obj in objects)
        {
            if (obj is not null) continue;
            await SendErrorPage(codeError, context);
            return false;
        }
        return true;
    }
    
    // TODO доделать обработку ошибок - ввел не число -> КАБУМ 
    // TODO проверять на то что приходит не NULL
    // Переделать с асинхронного на паралельный
    public async Task SendErrorPage(string codeError, HttpListenerContext context)
    {
        var currentPath = Directory.GetCurrentDirectory();
                    
        var pagePath = Path.Combine(currentPath , "static" , "statusPages", "errorPages", $"{codeError}.html");
            
        var response = context.Response;
        response.StatusCode = int.Parse(codeError);

        await FilePush(pagePath, response);
    }
    
}
using System.Net;

namespace http_server.Framework.Abstractions.Interfaces;

public interface IResourceService
{
    public Task<bool> GetPageNotFound(string filePath, HttpListenerContext context);
    public Task SendErrorPage(string codeError, HttpListenerContext context);
    
    public Task<bool> CheckNull(HttpListenerContext  context, string codeError, params object?[] objects);

}
using System.Net;

namespace MiniHttpServer.Framework.Framework.Abstractions;

public interface IResourceService
{
    public Task SendErrorPage(int codeError, HttpListenerContext context);
    public Task<bool> CheckNull(HttpListenerContext context, int codeError, params object?[] objects);

    public Task<bool> ResolveFile(string filePath, HttpListenerContext context);
}

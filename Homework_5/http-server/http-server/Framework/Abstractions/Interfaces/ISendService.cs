using System.Net;

namespace http_server.Framework.Abstractions.Interfaces;

public interface ISendMessageService
{ 
    Task SendMessageAsync(HttpListenerContext context);
}
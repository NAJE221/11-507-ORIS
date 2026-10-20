using System.Net;
using http_server.Framework.Abstractions.Interfaces;
using http_server.Framework.Attributes;

namespace http_server.Framework.HttpControllers;

[HttpController("home-work")]
public class HomeworkController(HttpListenerContext context, ISendMessageService service)
{
    [Get("send")]
    public async Task SendHomework()
    {
        await service.SendMessageAsync(context);
    }
}
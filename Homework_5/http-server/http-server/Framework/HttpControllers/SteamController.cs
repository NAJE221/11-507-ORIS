using System.Net;
using http_server.Framework.Abstractions.Interfaces;
using http_server.Framework.Attributes;

namespace http_server.Framework.HttpControllers;


// TODO Доделать проверку на пустой ввод 

[HttpController("steam")]
public class SteamController(HttpListenerContext context, ISendMessageService service)
{
    [Get("send")]
    public async Task SendHeroAsync()
    { 
        await service.SendMessageAsync(context);
    }
}
using System.Net;
using MiniHttpServer.Abstraction;
using MiniHttpServer.Framework.Framework.Attributes;
using MiniHttpServer.Framework.Framework.Services;
using MiniHttpServer.HtmlTemplating;

namespace MiniHttpServer.Controllers;

[HttpController("profile")]
public class TestController (HttpListenerResponse response, IUserService userService)
{
    [Get("")]
    public async Task GetUsers()
    {
        var listUsers = await userService.GetUsersAsync();

        foreach (var user in listUsers)
        {
            Console.WriteLine(user);
        }
    }
}

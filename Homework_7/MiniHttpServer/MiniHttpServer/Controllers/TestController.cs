using System.Net;
using MiniHttpServer.Framework.Framework.Attributes;
using MiniHttpServer.Framework.Framework.Services;
using MiniHttpServer.HtmlTemplating;
using MiniHttpServer.HtmlTemplating.Parsing;

namespace MiniHttpServer.Controllers;

[HttpController("profile")]
public class TestController (HttpListenerResponse response)
{
    [Get("")]
    public async Task GetProfile()
    {
        var lexer = new Lexer();
        var parser = new Parser();
        var renderer = new HtmlTemplateRenderer();

        var user = new User("Вася", 18, "penis@mail.ru");
        
        var currentPath = Directory.GetCurrentDirectory();
        var pathInput = Path.Combine(currentPath, "static", "templates", "profile.html");
        var pathOutput = Path.Combine(currentPath, "static", "templates", "profile.html");
        renderer.RenderToFile(pathInput,pathOutput,user);
        
        await FileService.FilePush(pathOutput, response);
    }
}

public class User(string name, int age,  string email)  
{
    public string name { get; } = name;
    public int age { get; } = age;
    public string email { get; } = email;
}
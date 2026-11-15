using MiniHttpServer.HtmlTemplating;
using System.Net;
using http_server.Framework.Attributes;
using http_server.Framework.Services;

namespace http_server.Framework.HttpControllers;

[HttpController("profile")]
public class TestController (HttpListenerResponse response)
{
    [Get("")]
    public async Task GetProfile()
    {
        var lexer = new Lexer();
        var parser = new Parser();
        var renderer = new HtmlTemplateRenderer(lexer, parser);

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
using System.Net;
using http_server.Framework.Utils;

namespace http_server.Framework.Http;

public class HttpServer
{
    private HttpListener _server = new();
    
    private void StartServer()
    {
        var prefix = DataDeserializer.Prefix;
        var server = prefix.Server;

        Uri uri = server.GetUri();
        var uriString = uri.ToString();
        
        _server.Prefixes.Add(uriString);

        while (!_server.IsListening)
        {
            ConsoleCommand.PrintMenu();
            ConsoleCommand.CommandProcessing(_server);
        }
        
        Console.Clear();
        Console.WriteLine($"Сервер запущен\n{uriString}");
    }

    private async Task AsyncListen()
    {
        try
        {
            while (_server.IsListening)
            {
                var context = await _server.GetContextAsync();
                await AsyncCreateResponse(context);
            }
        }
        catch (HttpListenerException)
        {
            Console.WriteLine("Ошибка обработана");
        }
    }

    private async Task AsyncCreateResponse(HttpListenerContext context)
    {
        var request = context.Request;
        Console.WriteLine("\nЗапрос пришел");
        Console.WriteLine(request.Url.LocalPath);
        
        var response = context.Response;

        var pathUser = request.Url.LocalPath.Trim('/');
        
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "static", pathUser);
        
        ResourceValidator.SendNotFoundPage(response , ref filePath);
        
        FileInfo fileInfo = new(filePath);
        
        MimeTypeChecker.CheckMimeType(response , fileInfo);
        
        byte[] buffer = await File.ReadAllBytesAsync(filePath);
        response.ContentLength64 = buffer.Length;
        using Stream output = response.OutputStream;
        
        await output.WriteAsync(buffer);
        await output.FlushAsync();
        
        Console.WriteLine("Запрос обработан");
    }


    public void Run()
    {

        StartServer();
        var taskCommandProcessing = Task.Run(() =>
        {
            while (_server.IsListening)
            {
                ConsoleCommand.CommandProcessing(_server);
            }
        });
        
        
        var taskAsyncListen = Task.Run(async () => { await AsyncListen(); });
        
        Task.WaitAll(taskAsyncListen, taskCommandProcessing);
    }
}
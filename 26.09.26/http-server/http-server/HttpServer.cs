using System.Net;
using System.Text;
using System.Text.Json;

namespace http_server;

public class HttpServer
{
    HttpListener _server = new();
    
    private async Task<string> ReadFile(string fileName)
    {
        var currentDict = Directory.GetCurrentDirectory();
        var path = Path.Combine(currentDict, fileName);
        string responseText;
        
        using (StreamReader reader = new StreamReader(path, Encoding.UTF8))
        {
            responseText = await reader.ReadToEndAsync();
        }
        
        Console.WriteLine("================\n" +
                          "html извлечен");
        return responseText;
    }
    
    private void StartServer()
    {
        var prefix = JsonSerializer.Deserialize<Server> (File.ReadAllText("server.json"));
        var url = $"http://{prefix.host}:{prefix.port}/{prefix.path}";
        _server.Prefixes.Add(url);
        _server.Start();
        Console.WriteLine($"Сервер запущен\n{url}");
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
            Console.WriteLine("Сервер закрылся");
        }
    }

    private async Task AsyncCreateResponse(HttpListenerContext context)
    {
        var response = context.Response;
        var responseText = await ReadFile("index.html");
        byte[] buffer = Encoding.UTF8.GetBytes(responseText);
        response.ContentLength64 = buffer.Length;

        using Stream output = response.OutputStream;

        await output.WriteAsync(buffer);
        await output.FlushAsync();
        
        Console.WriteLine("Запрос обработан" +
                          "\n================");
    }


    public void Run()
    {
        StartServer();
        var taskAsyncListen = Task.Run(async () => { await AsyncListen(); });
        
        var taskCommandProcessing = Task.Run(() =>
        {
            while (_server.IsListening)
            {
                ConsoleCommand.CommandProcessing(_server);
            }
        });
        
        Task.WaitAll(taskAsyncListen, taskCommandProcessing);
    }
}
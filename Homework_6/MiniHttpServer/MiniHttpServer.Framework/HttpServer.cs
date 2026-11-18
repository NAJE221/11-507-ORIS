using System.Net;
using MiniHttpServer.Framework.Framework.Abstractions;
using MiniHttpServer.Framework.Framework.Terminal;

namespace MiniHttpServer.Framework;

public class HttpServer (
    HttpListener listener, 
    Handler handler)
{
    
    private void StartServer()
    {
        while (!listener.IsListening)
        {
            TerminalUi.PrintMenu();
            TerminalCommand.CommandProcessing(listener);
        }
    }

    private async Task ListenAsync()
    {
        try
        {
            while (listener.IsListening)
            {
                var context = await listener.GetContextAsync(); 
                _ = ProcessRequestAsync(context);
            }
        }
        catch (HttpListenerException)
        {
            Console.WriteLine("Ошибка обработана");
        }
        finally
        {
            listener.Close();
        }
    }

    private async Task ProcessRequestAsync(HttpListenerContext context)
    {
        try
        {
            await handler.HandlerRequest(context);
            context.Response.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка обработки запроса: {ex}");
            TrySendError(context);
        }
    }

    private static void TrySendError(HttpListenerContext context)
    {
        try
        {
            context.Response.StatusCode = 500;
            context.Response.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Не удалось отправить ответ: {ex.Message}");
        }
    }

    private void ListenTerminal()
    {
        while (listener.IsListening)
        {
            TerminalCommand.CommandProcessing(listener);
        }
    }
    
    public async Task RunAsync()
    {
        StartServer();

        var taskCommandProcessing = Task.Run(ListenTerminal);
        var taskListenAsync = ListenAsync();
        
        await Task.WhenAll(taskListenAsync, taskCommandProcessing);
    }
}
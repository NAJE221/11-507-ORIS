using System.Net;
namespace http_server;

public static class ConsoleCommand
{

    public static string InputCommand()
    {
        var command = Console.ReadLine().ToLower().Trim();
        return command;
    }
    
    public static void CommandProcessing(HttpListener server)
    {
        var command = InputCommand();
        
        switch (command)
        {
            case "stop":
                try
                {
                    server.Stop();
                    Console.WriteLine("Программа остановлена");
                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine($"Сервер еще не запущен!\n" +
                                      $"Чтобы запустить, введите - start");
                }
                break;
        }
    }
}
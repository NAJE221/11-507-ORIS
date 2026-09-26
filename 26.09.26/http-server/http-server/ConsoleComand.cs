using System.Globalization;
using System.Net;
namespace http_server;

public static class ConsoleCommand
{

    public static void PrintMenu()
    {
        Console.Clear();
        Console.WriteLine("Доступные команды:\n" +
                          "start - запустить сервер\n" +
                          "stop - остановить сервер\n");
        
    }
    

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
            
            case "start":
                
                Console.WriteLine(server.IsListening ? "Сервер уже запущен" : "\nСервер успешно запущен");
                if (!server.IsListening) server.Start();
                
                break;
            
            case "stop":
                try
                {
                    Console.WriteLine(server.IsListening ? "\nСервер остановлен" : "\nСервер не запущен!");
                    if (server.IsListening) server.Stop();
                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine($"Ошибка обработана");
                }
                break;
        }
    }
}
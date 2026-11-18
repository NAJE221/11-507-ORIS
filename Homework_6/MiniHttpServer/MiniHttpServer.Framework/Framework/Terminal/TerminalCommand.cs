using System.Net;

namespace MiniHttpServer.Framework.Framework.Terminal;

public static class TerminalCommand
{
    public static void CommandProcessing(
        HttpListener listener)
    {
        var command = Console.ReadLine()?
            .ToLower()
            .Trim();
        
        switch (command)
        {
            case "start":
                
                if (listener.IsListening)
                    Console.WriteLine("Сервер уже запущен");
                
                else
                {
                    listener.Start();
                    TerminalUi.PrintSuccessfulStart(listener);
                }
                break;
            
            case "stop":
                
                if (!listener.IsListening)
                    Console.WriteLine("Сервер уже остановлен");
                
                else
                {
                    listener.Stop();
                    Console.WriteLine("Сервер остановлен");
                }
                break;
        }
        
    }
}
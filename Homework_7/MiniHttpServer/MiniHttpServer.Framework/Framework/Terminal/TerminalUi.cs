using System.Net;

namespace MiniHttpServer.Framework.Framework.Terminal;

public class TerminalUi
{
    public static void PrintMenu()
    {
        Console.Clear();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("""
                           ╔══════════════════════════════════════╗
                           ║                                      ║
                           ║          MINI HTTP SERVER            ║
                           ║                                      ║
                           ╠══════════════════════════════════════╣
                           ║                                      ║
                           ║   [1]  START   - Запустить сервер    ║
                           ║   [2]  STOP    - Остановить сервер   ║
                           ║                                      ║
                           ╚══════════════════════════════════════╝
                          """);

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("Введите команду: ");
        Console.ResetColor();
    }

    public static void PrintPrefixes(HttpListener listener)
    {
        foreach (var prefix in listener.Prefixes)
        {
            Console.WriteLine(prefix);
        }
    }

    public static void PrintSuccessfulStart(HttpListener listener)
    {
        Console.Clear();
        Console.WriteLine("Сервер запущен");
        TerminalUi.PrintPrefixes(listener);
    }
}
using System.Net;
using System.Text;
using http_server.Framework.Http;

namespace http_server.Framework.Services;

public static class FileService
{
    public static async Task FilePush(string filePath, HttpListenerResponse response)
    {
        var buffer = await File.ReadAllBytesAsync(filePath);
        FileInfo fileInfo = new(filePath);
        MimeTypeChecker.CheckMimeType(response , fileInfo);
        
        await Push(buffer, response);
    }

    public static async Task PagePush(string str, HttpListenerResponse response)
    {
        var buffer = Encoding.UTF8.GetBytes(str);
        response.ContentType = "text/plain; charset=utf-8";
        
        await Push(buffer, response);
    }

    private static async Task Push(byte[] buffer , HttpListenerResponse response)
    {
        response.ContentLength64 = buffer.Length;
        await using Stream output = response.OutputStream;
        await output.WriteAsync(buffer);
        await output.FlushAsync();
    }
}
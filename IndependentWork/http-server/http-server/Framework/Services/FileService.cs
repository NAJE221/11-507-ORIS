using System.Net;
using http_server.Framework.Http;

namespace http_server.Framework.Services;

public static class FileService
{
    public static async Task FilePush(string filePath, HttpListenerResponse response)
    {
        var buffer = await File.ReadAllBytesAsync(filePath);
        var output = response.OutputStream;
        response.ContentLength64 = buffer.Length;
        
        FileInfo fileInfo = new(filePath);
        MimeTypeChecker.CheckMimeType(response , fileInfo);
        
        await output.WriteAsync(buffer);
        await output.FlushAsync();
    }
}
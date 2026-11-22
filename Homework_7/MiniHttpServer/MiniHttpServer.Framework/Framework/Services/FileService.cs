using System.Net;

namespace MiniHttpServer.Framework.Framework.Services;

public static class FileService
{
    public static async Task FilePush(string filePath, HttpListenerResponse response)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentNullException(nameof(filePath));
        
        var buffer = await File.ReadAllBytesAsync(filePath);
        FileInfo fileInfo = new(filePath);
        MimeTypeCheckerService.CheckMimeType(response , fileInfo);
        
        await Push(buffer, response);
    }
    
    public static async Task FilePush(
        byte[] buffer,
        string contentType,
        HttpListenerResponse response)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        ArgumentException.ThrowIfNullOrWhiteSpace(contentType);

        response.ContentType = contentType;

        await Push(buffer, response);
    }

    private static async Task Push(byte[] buffer , HttpListenerResponse response)
    {
        response.ContentLength64 = buffer.Length;
        await using var output = response.OutputStream;
        await output.WriteAsync(buffer);
        await output.FlushAsync();
    }
}
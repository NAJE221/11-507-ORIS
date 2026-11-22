using System.Net;
using MiniHttpServer.Framework.Framework.Abstractions;
using MiniHttpServer.Framework.Framework.Enums;
using static MiniHttpServer.Framework.Framework.Services.FileService;

namespace MiniHttpServer.Framework.Framework.Services;

public class ResourceService(string errorPagesDirectory = "DefaultErrorsPages") 
    : IResourceService
{
    public async Task<bool> CheckNull(
        HttpListenerContext context,
        int codeError,
        params object?[] objects)
    {
        foreach (var obj in objects)
        {
            if (obj is not null) continue;

            await SendErrorPage(codeError, context);
            return false;
        }

        return true;
    }

    public async Task<bool> ResolveFile(
        string filePath,
        HttpListenerContext context)
    {
        if (File.Exists(filePath)) return true;

        await SendErrorPage(404, context);
        return false;
    }

    public async Task SendErrorPage(
        int codeError,
        HttpListenerContext context)
    {
        if (!ErrorCodesEnum.ValidHttpErrors.Contains(codeError))
            throw new Exception($"Код ошибки: {codeError} не существует!");

        var assembly = typeof(ResourceService).Assembly;

        var resourceName =
            $"MiniHttpServer.Framework.static.DefaultErrorsPages.{codeError}.html";

        await using var stream = assembly.GetManifestResourceStream(resourceName);

        if (stream is null)
            throw new FileNotFoundException(
                $"Встроенная страница ошибки {codeError} не найдена!");

        await using var memoryStream = new MemoryStream();

        await stream.CopyToAsync(memoryStream);

        var response = context.Response;
        response.StatusCode = codeError;

        await FilePush(
            memoryStream.ToArray(),
            "text/html; charset=utf-8",
            response);
    }
}
namespace http_server.Framework.Abstractions;

public interface IResourceValidator
{
    bool IsValid(string path);
}
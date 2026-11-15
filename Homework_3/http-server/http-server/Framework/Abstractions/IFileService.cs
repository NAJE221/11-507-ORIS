using System.Net;

namespace http_server.Framework.Abstractions;

public interface IFileService
{ 
    Task FilePush(string filePath, HttpListenerResponse response);
}
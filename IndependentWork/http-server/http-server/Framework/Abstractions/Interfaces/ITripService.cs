using System.Net;

namespace http_server.Framework.Abstractions.Interfaces;

public interface ITripService
{ 
    Task GetTripById(HttpListenerContext context);
}
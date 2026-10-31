using System.Net;
using http_server.Framework.DTOs;

namespace http_server.Framework.Factories;

public static class TripsRequestMapper
{
    public static TripsRequest Map(HttpListenerContext context)
    {
        var query = context.Request.QueryString;
        var emailSendRequest = new TripsRequest(
            id       : int.Parse(query["id"])
        );
        return emailSendRequest;
    }
}
using System.Net;
using http_server.Framework.DTOs;

namespace http_server.Framework.Factories;

public static class EmailRequestMapper
{
    public static EmailSendRequest Map(HttpListenerContext context)
    {
        var query = context.Request.QueryString;
        var emailSendRequest = new EmailSendRequest(
            to       : query["to"] ?? "",
            subject  : query["subject"] ?? "",
            text     : query["text"] ?? ""
            
        );
        return emailSendRequest;
    }
}
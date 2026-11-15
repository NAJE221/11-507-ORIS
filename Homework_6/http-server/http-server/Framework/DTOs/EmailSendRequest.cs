namespace http_server.Framework.DTOs;

public record EmailSendRequest(
    string to,
    string subject, 
    string text);

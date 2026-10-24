using System.Net;
using http_server.Framework.Abstractions.Interfaces;
using http_server.Framework.Factories;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using static http_server.Framework.Services.FileService;

namespace http_server.Framework.Services;

public class EmailService(IResourceService resourceService) : ISendMessageService
{
    private static MimeMessage CreateMessage(HttpListenerContext context)
    {
      
        var emailSendRequest = EmailRequestMapper.Map(context);
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress("Артём", "artemigorevich113@gmail.com"));
        message.To.Add(new MailboxAddress("Получатель", emailSendRequest.to));

        message.Subject = "Домашняя работа Юнусов Артём Игоревич 11-507";

        var builder = new BodyBuilder
        {
            TextBody = "https://disk.360.yandex.ru/d/iTpxQeWJESk8mQ"
        };


        message.Body = builder.ToMessageBody();
        
        return message;
    }

 

    public async Task SendMessageAsync(HttpListenerContext context)
    {
        
        try
        {
            var message = CreateMessage(context);
            using var client = new SmtpClient();

            await client.ConnectAsync("smtp.gmail.com", 587 , SecureSocketOptions.StartTls);
            await client.AuthenticateAsync("artemigorevich113@gmail.com", "тут пароль");
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
         
            
            await FilePush(@"C:\Users\Artem\Desktop\11-507-ORIS\Homework_4\http-server\http-server\static\statusPages\successfulPages\email-sent.html", context.Response);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Email] Ошибка: {ex.Message}");
            await resourceService.SendErrorPage("500", context);
        }
    }
}
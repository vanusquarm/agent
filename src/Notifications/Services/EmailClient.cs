using System.Net.Mail;
//using Serilog;

namespace Notification.Services;

public class EmailClient
{
    private readonly IConfiguration _configuration;
    private readonly SmtpClient _smtpClient;

    public EmailClient(IConfiguration configuration)
    {
        _configuration = configuration;
        var mail = _configuration.GetSection("Mail");
        _smtpClient = new SmtpClient(mail["Host"], Convert.ToInt32(mail["Port"]));
    }

    public void SendEmail(string to, string body, string subject)
    {
        var mail = _configuration.GetSection("Mail");
        var from = mail["From"];
        var emailList = to.Split(';');

        MailMessage message = new()
        {
            From = new MailAddress(from),
            IsBodyHtml = true,
            Subject = subject,
            Body = body,
        };
        foreach (var email in emailList)
        {
            message.To.Add(email);
        }

        _smtpClient.Send(message);
    }

    public void SendEmailWithAttachment(string to, string body, string subject, Attachment file)
    {
        try
        {
            var mail = _configuration.GetSection("Mail");
            var from = mail["From"];
            MailMessage message = new(from, to, subject, body)
            {
                IsBodyHtml = true
            };
            message.Attachments.Add(file);
            _smtpClient.Send(message);
        }
        catch (Exception ex)
        {
            // Log the exception
            //Log.Error(ex, "An error occurred while sending email");
        }
    }

}
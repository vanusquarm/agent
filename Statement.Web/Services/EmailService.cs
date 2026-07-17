using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using Statement.Web.Interfaces;
using Statement.Web.Models;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace Statement.Web.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly string _smtpServer;
        private readonly ILogger<EmailService> _logger;
        private readonly int _smtpPort;
        private readonly bool _enableSsl;
        private readonly string? _smtpUsername;
        private readonly string? _smtpPassword;
        private readonly EmailSettings _settings;


        private readonly IWebHostEnvironment _environment;
        public EmailService(IConfiguration configuration, ILogger<EmailService> logger, IWebHostEnvironment environment, IOptions<EmailSettings> settings)
        {
            _configuration = configuration;

            _logger = logger;
            _smtpServer = configuration["EmailSettings:SmtpServer"]!;
            _smtpPort = configuration.GetValue<int>("EmailSettings:SmtpPort", 587);
            _enableSsl = configuration.GetValue<bool>("EmailSettings:EnableSsl", true);
            _smtpUsername = configuration["EmailSettings:SmtpUsername"];
            _smtpPassword = configuration["EmailSettings:SmtpPassword"];
            _environment = environment;
            _settings = settings.Value;
        }
  

        public bool SendIntroHtmlMessage(
            string sendTo,
            string sendFrom,
            string sendSubject,
            string customerName,
            string statementPeriod,
            string attachmentPath,
            string accountNumToEmail,
            string productToEmail)
        {
            _logger.LogInformation(
                "Inside SendIntroHtmlMessage. Recipient: {Recipient}",
                sendTo);

            try
            {
                string htmlMessage = $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                    <style>
                    body {{
                        font-family: Arial, sans-serif;
                        line-height:1.6;
                        color:#333;
                        max-width:800px;
                        margin:0 auto;
                    }}
                    .header {{
                        display:flex;
                        flex-direction:column;
                        align-items:flex-end;
                        margin-bottom:20px;
                    }}
                    .logo {{
                        height:60px;
                    }}
                    .date {{
                        font-size:14px;
                        color:#999;
                        margin-top:5px;
                    }}
                    .content {{
                        padding:15px;
                    }}
                    .footer {{
                        margin-top:30px;
                        font-size:12px;
                        color:#666;
                        text-align:center;
                        border-top:1px solid #eee;
                        padding-top:20px;
                    }}
                    .banner-container {{
                        margin-top:40px;
                        text-align:center;
                    }}
                    .banner {{
                        width:100%;
                        max-width:600px;
                    }}
                    .highlight {{
                        color:#0066cc;
                        font-weight:bold;
                    }}
                    </style>
                    </head>

                    <body>

                    <div class='header'>
                    <img src='cid:gtblogo' class='logo'/>
                    <div class='date'>{DateTime.Now:dddd, dd MMMM yyyy}</div>
                    </div>

                    <div class='content'>

                    <p>Dear <span class='highlight'>{customerName.ToUpper()}</span>,</p>

                    <p>
                    STATEMENT BY EMAIL FOR
                    <span class='highlight'>{statementPeriod}</span>
                    </p>

                    <p>Please find attached your GTBank E-statement.</p>

                    <p>
                    For security purposes, the file has been protected with a password.
                    To open the attachment, kindly enter the
                    <span class='highlight'>
                    last 6 digits of your {productToEmail} -
                    {accountNumToEmail[^6..]}
                    </span>
                    as the password.
                    </p>

                    <p>
                    If you are viewing this email on a mobile phone or iPad,
                    please save the document first before opening it.
                    </p>

                    <p>
                    For enquiries please contact us on
                    (+233 302) 675224, 680746, 687751, 680662
                    or email Corporate Affairs.
                    </p>

                    </div>

                    <div class='banner-container'>
                    <img src='cid:advert' class='banner'/>
                    </div>

                    <div class='footer'>
                    <p>
                    This email and its attachments are confidential.
                    </p>

                    <p>
                    © {DateTime.Now.Year} GTBank.
                    </p>

                    </div>

                    </body>
                    </html>";

                using var message = new MailMessage
                {
                    Subject = sendSubject,
                    From = new MailAddress(sendFrom, "GTBank E-Statement"),
                    IsBodyHtml = true
                };

                foreach (var address in sendTo.Replace(",", ";")
                                              .Split(';', StringSplitOptions.RemoveEmptyEntries))
                {
                    if (ValidateEmailAddress(address.Trim()))
                    {
                        message.To.Add(address.Trim());
                    }
                }

                if (!string.IsNullOrWhiteSpace(attachmentPath))
                {
                    if (File.Exists(attachmentPath))
                    {
                        message.Attachments.Add(new Attachment(attachmentPath));

                        _logger.LogInformation(
                            "Attachment added: {Path}",
                            attachmentPath);
                    }
                    else
                    {
                        _logger.LogWarning(
                            "Attachment not found: {Path}",
                            attachmentPath);
                    }
                }

                var htmlView = AlternateView.CreateAlternateViewFromString(
                    htmlMessage,
                    null,
                    MediaTypeNames.Text.Html);

                string resourcesFolder = Path.Combine(
                    _environment.ContentRootPath,
                    "Resources");

                string logoPath = Path.Combine(resourcesFolder, "glogo.jpg");
                string advertPath = Path.Combine(resourcesFolder, "gtco_ad.jpeg");

                if (File.Exists(logoPath))
                {
                    var logo = new LinkedResource(logoPath)
                    {
                        ContentId = "gtblogo"
                    };

                    htmlView.LinkedResources.Add(logo);

                    _logger.LogInformation(
                        "Logo linked successfully.");
                }
                else
                {
                    _logger.LogWarning(
                        "Logo not found at {Path}",
                        logoPath);
                }

                if (File.Exists(advertPath))
                {
                    var advert = new LinkedResource(advertPath)
                    {
                        ContentId = "advert"
                    };

                    htmlView.LinkedResources.Add(advert);

                    _logger.LogInformation(
                        "Advertisement linked successfully.");
                }
                else
                {
                    _logger.LogWarning(
                        "Advertisement image not found at {Path}",
                        advertPath);
                }

                message.AlternateViews.Add(htmlView);

                using var smtp = new System.Net.Mail.SmtpClient(_smtpServer)
                {
                    Port = _smtpPort,
                    EnableSsl = _enableSsl,
                    UseDefaultCredentials = false
                };

                if (!string.IsNullOrWhiteSpace(_smtpUsername))
                {
                    smtp.Credentials = new NetworkCredential(
                        _smtpUsername,
                        _smtpPassword);
                }

                smtp.Send(message);

                _logger.LogInformation(
                    "Successfully sent email to {Recipient}",
                    sendTo);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error sending statement email to {Recipient}",
                    sendTo);

                return false;
            }
        }
        public string SendMessage(
            string sendTo,
            string? copyTo,
            string sendFrom,
            string sendSubject,
            string sendMessage)
        {
            if (string.IsNullOrWhiteSpace(_smtpServer))
            {
                _logger.LogError("SMTP server is not configured.");

                return "SMTP server not configured.";
            }

            try
            {
                _logger.LogInformation(
                    "Attempting to send email to {Recipient}. Subject: {Subject}",
                    sendTo,
                    sendSubject);

                if (!ValidateEmailAddress(sendTo))
                {
                    _logger.LogWarning(
                        "Invalid recipient email address: {Recipient}",
                        sendTo);

                    return $"Invalid recipient email address: {sendTo}";
                }

                using var message = new MailMessage
                {
                    From = new MailAddress(sendFrom),
                    Subject = sendSubject,
                    Body = sendMessage,
                    IsBodyHtml = true
                };

                message.To.Add(sendTo);

                if (!string.IsNullOrWhiteSpace(copyTo))
                {
                    try
                    {
                        message.CC.Add(copyTo);
                    }
                    catch (FormatException ex)
                    {
                        _logger.LogWarning(
                            ex,
                            "Invalid CC email address: {CC}",
                            copyTo);

                        return $"Invalid CC email address: {copyTo}";
                    }
                }

                using var smtpClient = new System.Net.Mail.SmtpClient(_smtpServer)
                {
                    Port = _smtpPort,
                    EnableSsl = _enableSsl,
                    UseDefaultCredentials = false
                };

                if (!string.IsNullOrWhiteSpace(_smtpUsername))
                {
                    smtpClient.Credentials = new NetworkCredential(
                        _smtpUsername,
                        _smtpPassword);
                }

                smtpClient.Send(message);

                var result =
                    $"Message sent successfully to {sendTo} at {DateTime.Now:yyyy-MM-dd HH:mm:ss}";

                _logger.LogInformation(result);

                return result;
            }
            catch (SmtpException ex)
            {
                _logger.LogError(
                    ex,
                    "SMTP error sending email to {Recipient}",
                    sendTo);

                return $"SMTP error: {ex.Message}";
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error sending email to {Recipient}",
                    sendTo);

                return ex.Message;
            }
        }

        public bool ValidateEmailAddress(string emailAddress)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(emailAddress.Trim());
                return addr.Address == emailAddress.Trim();
            }
            catch
            {
                return false;
            }
        }
    }
}

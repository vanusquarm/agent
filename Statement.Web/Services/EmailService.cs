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
        //private readonly string? _smtpUsername;
        //private readonly string? _smtpPassword;
        private readonly EmailSettings _settings;

        private readonly IWebHostEnvironment _environment;
        public EmailService(IConfiguration configuration, ILogger<EmailService> logger, IWebHostEnvironment environment, IOptions<EmailSettings> settings)
        {
            _configuration = configuration;

            _logger = logger;
            _smtpServer = configuration["EmailSettings:SmtpServer"]!;
            _smtpPort = configuration.GetValue<int>("EmailSettings:SmtpPort", 25);
            _enableSsl = configuration.GetValue<bool>("EmailSettings:EnableSsl", true);
            //_smtpUsername = configuration["EmailSettings:SmtpUsername"];
            //_smtpPassword = configuration["EmailSettings:SmtpPassword"];
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
            _logger.LogInformation("Inside SendIntroHtmlMessage. Recipient: {Recipient}", sendTo);

            try
            {
                string htmlMessage = $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <style>
                            body {{ 
                                font-family: Arial, sans-serif; 
                                line-height: 1.6; 
                                color: #333; 
                                max-width: 800px; 
                                margin: 0 auto; 
                            }}
                            .header {{ 
                                display: flex; 
                                flex-direction: column; 
                                align-items: flex-end; 
                                margin-bottom: 20px; 
                            }}
                            .logo {{ 
                                height: 60px; 
                            }}
                            .date {{
                                font-size: 14px;
                                color: #999;
                                margin-top: 5px;
                            }}
                            .content {{ 
                                padding: 15px; 
                            }}
                            .footer {{ 
                                margin-top: 30px; 
                                font-size: 12px; 
                                color: #666; 
                                text-align: center;
                                border-top: 1px solid #eee;
                                padding-top: 20px;
                            }}
                            .banner-container {{ 
                                margin-top: 40px;
                                text-align: center;
                            }}
                            .banner {{ 
                                width: 100%; 
                                max-width: 600px;
                            }}
                            .highlight {{ 
                                color: #0066cc; 
                                font-weight: bold; 
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='header'>
                            <img src='cid:gtblogo' class='logo' alt='GTBank Logo'/>
                            <div class='date'>{DateTime.Now:dddd, dd MMMM yyyy}</div>
                        </div>

                        <div class='content'>
                            <p>Dear <span class='highlight'>{customerName.ToUpper()}</span>,</p>

                            <p>STATEMENT BY EMAIL FOR <span class='highlight'>{statementPeriod}</span></p>

                            <p>Please find attached your GTBank E-statement.</p>

                            <p>For security purposes, the file has been protected with a password. 
                            To open the attachment, kindly enter the <span class='highlight'>last 6 digits of your {productToEmail} - {GetLastSixDigits(accountNumToEmail)}</span> as the password.</p>

                            <p>If you are viewing this email on a mobile phone or iPad, please save the document first before opening it.</p>

                            <p>For enquiries please contact us on:<br>
                            <span class='highlight'>(+233 302) 675224, 680746, 687751, 680662</span><br>
                            or email <a href='mailto:corporateaffairs@gtbank.com'>Corporate Affairs</a>.</p>
                        </div>

                        <div class='banner-container'>
                            <img src='cid:advert' class='banner' alt='Advertisement Banner'/>
                        </div>

                        <div class='footer'>
                            <p>This email and its attachments are confidential.</p>
                            <p>© {DateTime.Now.Year} GTBank. All rights reserved.</p>
                        </div>
                    </body>
                    </html>";

                // Create the email message
                using var message = new MailMessage();
                message.Subject = sendSubject;
                message.From = new MailAddress(sendFrom, "GTBank E-Statement");
                message.IsBodyHtml = true;

                // Add recipients
                if (!string.IsNullOrWhiteSpace(sendTo))
                {
                    foreach (string addr in sendTo.Replace(",", ";").Split(';', StringSplitOptions.RemoveEmptyEntries))
                    {
                        string trimmedAddr = addr.Trim();
                        if (ValidateEmailAddress(trimmedAddr))
                        {
                            message.To.Add(new MailAddress(trimmedAddr));
                        }
                    }
                }

                // Create HTML view with embedded images
                var htmlView = AlternateView.CreateAlternateViewFromString(
                    htmlMessage,
                    null,
                    MediaTypeNames.Text.Html);

                // Get the Resources folder path
                string resourcesFolder = Path.Combine(_environment.ContentRootPath, "Resources");

                // Add logo image
                string logoPath = Path.Combine(resourcesFolder, "glogo.jpg");
                if (File.Exists(logoPath))
                {
                    var logo = new LinkedResource(logoPath, "image/jpg");
                    logo.ContentId = "gtblogo";
                    htmlView.LinkedResources.Add(logo);
                    _logger.LogInformation("Logo linked successfully: {Path}", logoPath);
                }
                else
                {
                    _logger.LogWarning("Logo not found at: {Path}", logoPath);
                }

                // Add advertisement image
                string advertPath = Path.Combine(resourcesFolder, "gtco_ad.jpeg");
                if (File.Exists(advertPath))
                {
                    var advert = new LinkedResource(advertPath, "image/jpeg");
                    advert.ContentId = "advert";
                    htmlView.LinkedResources.Add(advert);
                    _logger.LogInformation("Advertisement linked successfully: {Path}", advertPath);
                }
                else
                {
                    _logger.LogWarning("Advertisement image not found at: {Path}", advertPath);
                }

                message.AlternateViews.Add(htmlView);

                // Add attachment if provided
                if (!string.IsNullOrWhiteSpace(attachmentPath))
                {
                    if (File.Exists(attachmentPath))
                    {
                        var attachment = new Attachment(attachmentPath, MediaTypeNames.Application.Octet);
                        attachment.Name = Path.GetFileName(attachmentPath);
                        message.Attachments.Add(attachment);
                        _logger.LogInformation("Attachment added: {Path}", attachmentPath);
                    }
                    else
                    {
                        _logger.LogWarning("Attachment not found: {Path}", attachmentPath);
                    }
                }

                using (var smtp = new System.Net.Mail.SmtpClient(_smtpServer, _smtpPort))
                {
                    smtp.UseDefaultCredentials = false;

                    smtp.EnableSsl = _enableSsl;

                    smtp.Timeout = 30000;

                    smtp.Send(message);

                    _logger.LogInformation("Successfully sent email to {Recipient}", sendTo);

                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error sending statement email to {Recipient}: {ErrorMessage}",
                    sendTo,
                    ex.Message);

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
                    From = new MailAddress(sendFrom, "GTBank System Notification"),
                    Subject = sendSubject,
                    Body = sendMessage,
                    IsBodyHtml = true
                };

                // Add primary recipient
                message.To.Add(sendTo);

                // Add CC if provided
                if (!string.IsNullOrWhiteSpace(copyTo))
                {
                    try
                    {
                        // Handle multiple CC addresses separated by comma or semicolon
                        foreach (string ccAddr in copyTo.Replace(",", ";").Split(';', StringSplitOptions.RemoveEmptyEntries))
                        {
                            string trimmedCc = ccAddr.Trim();
                            if (ValidateEmailAddress(trimmedCc))
                            {
                                message.CC.Add(trimmedCc);
                            }
                            else
                            {
                                _logger.LogWarning("Invalid CC email address: {CC}", trimmedCc);
                            }
                        }
                    }
                    catch (FormatException ex)
                    {
                        _logger.LogWarning(
                            ex,
                            "Invalid CC email address format: {CC}",
                            copyTo);
                        return $"Invalid CC email address format: {copyTo}";
                    }
                }

                // ============ KEY CHANGE: SMTP Client without authentication ============
                using var smtpClient = new System.Net.Mail.SmtpClient(_smtpServer)
                {
                    Port = _smtpPort,
                    EnableSsl = _enableSsl,
                    UseDefaultCredentials = false  // No credentials - same as .NET Framework
                };

                // Note: We're NOT setting Credentials here
                // This matches the .NET Framework behavior where no authentication is used

                // Send the email
                smtpClient.Send(message);

                var result = $"Message sent successfully to {sendTo} at {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                _logger.LogInformation(result);

                return result;
            }
            catch (SmtpException ex)
            {
                _logger.LogError(
                    ex,
                    "SMTP error sending email to {Recipient}. Error: {ErrorMessage}",
                    sendTo,
                    ex.Message);
                return $"SMTP error: {ex.Message}";
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error sending email to {Recipient}. Error: {ErrorMessage}",
                    sendTo,
                    ex.Message);
                return $"Unexpected error: {ex.Message}";
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
        private string GetLastSixDigits(string accountNumber)
        {
            if (string.IsNullOrEmpty(accountNumber))
                return "******";

            // For display: show first 3 digits and mask the rest
            if (accountNumber.Length >= 6)
            {
                string lastSix = accountNumber.Substring(accountNumber.Length - 6);
                return $"{lastSix.Substring(0, 3)}**********";
            }
            else
            {
                return accountNumber.PadLeft(6, '*');
            }
        }
    }
}

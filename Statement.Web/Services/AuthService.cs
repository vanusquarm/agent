using Statement.Web.Controllers;
using Statement.Web.Interfaces;
using Statement.Web.Models;
using System.ServiceModel;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Statement.Web.Services
{
    public class AuthService : IAuthService
    {
        private readonly ILogger<AuthService> _logger;
        private readonly IConfiguration _configuration;
        public async Task<AuthResponse> ValidateUserAsync(string uid, string upass)
        {
            try
            {
                var appId = _configuration.GetValue<int>("ApplicationId", 45);

                ///string soapResponse = await _soapClient.ValidateAdminUserAsync(uid, upass, appId);
                string soapResponse = "";

                _logger.LogDebug("SSO validation for {UserId} with appId {AppId}", uid, appId);

                var document = XDocument.Parse(soapResponse);
                var ns = document.Root?.Name.Namespace;

                var codeNode = document.XPathSelectElement("/Response/CODE", ns != null ? new XmlNamespaceManager(new NameTable()) : null);

                if (codeNode?.Value != "1000")
                {
                    var errorNode = document.XPathSelectElement("/Response/ERROR");
                    var errorMessage = errorNode?.Value ?? "Authentication failed";

                    _logger.LogError("Authentication failed for user {UserId}: {ErrorMessage}", uid, errorMessage);

                    return new AuthResponse
                    {
                        Success = false,
                        Message = errorMessage
                    };
                }

                // Extract user data
                var result = new AuthResponse
                {
                    Success = true,
                    UserId = document.XPathSelectElement("/Response/USER/ID")?.Value,
                    UserName = document.XPathSelectElement("/Response/USER/NAME")?.Value,
                    BranchCode = document.XPathSelectElement("/Response/USER/BRANCH")?.Value,
                    Email = document.XPathSelectElement("/Response/USER/EMAIL")?.Value,
                    RoleId = document.XPathSelectElement("/Response/ROLE/RID")?.Value,
                    RawResponse = soapResponse
                };

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating user {UserId}", uid);
                return new AuthResponse
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
    }
}

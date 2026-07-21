using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Statement.Web.Models;
using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using System.Xml;

namespace Statement.Web.Services
{
    public interface ISSOService
    {
        Task<AuthResult> ValidateUserAsync(string uid, string upass);
    }

    public class SSOService : ISSOService
    {
        private readonly ILogger<SSOService> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _soapEndpointUrl;

        public SSOService(ILogger<SSOService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _soapEndpointUrl = _configuration["SSO:ServiceUrl"] ?? "http://10.61.223.97/GTBTECHAPPDEV_USA_NEW/AppDevService.asmx";
        }

        public async Task<AuthResult> ValidateUserAsync(string uid, string upass)
        {
            try
            {
                var appId = _configuration.GetValue<int>("SSO:ApplicationId", 45);

                string soapRequest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
                    <soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" 
                                   xmlns:tem=""http://tempuri.org/"">
                        <soap:Header/>
                        <soap:Body>
                            <tem:ValidateAdminUser>
                                <tem:uid>{uid}</tem:uid>
                                <tem:upass>{upass}</tem:upass>
                                <tem:appId>{appId}</tem:appId>
                            </tem:ValidateAdminUser>
                        </soap:Body>
                    </soap:Envelope>";

                // Create HTTP request
                var httpRequest = (HttpWebRequest)WebRequest.Create(_soapEndpointUrl);
                httpRequest.Method = "POST";
                httpRequest.ContentType = "text/xml; charset=utf-8";
                httpRequest.Headers.Add("SOAPAction", "http://tempuri.org/ValidateAdminUser");

                // Write request
                using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
                {
                    await streamWriter.WriteAsync(soapRequest);
                }

                // Get response
                using (var httpResponse = (HttpWebResponse)await httpRequest.GetResponseAsync())
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    string soapResponse = await streamReader.ReadToEndAsync();

                    _logger.LogDebug("SSO validation for {UserId} with appId {appId}", uid, appId);

                    return ParseSoapResponse(soapResponse, uid);
                }
            }
            catch (WebException ex)
            {
                _logger.LogError(ex, "Web exception for {UserId}: {Message}", uid, ex.Message);
                return new AuthResult
                {
                    Success = false,
                    Message = $"Service unavailable: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating user {UserId}: {Message}", uid, ex.Message);
                return new AuthResult { Success = false, Message = ex.Message };
            }
        }

        private AuthResult ParseSoapResponse(string soapResponse, string userId)
        {
            try
            {
                var document = new XmlDocument();
                document.LoadXml(soapResponse);

                var navigator = document.CreateNavigator();

                // Handle namespaces
                var nsManager = new XmlNamespaceManager(document.NameTable);
                nsManager.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
                nsManager.AddNamespace("tem", "http://tempuri.org/");

                // Try to find response with or without namespace
                var responseNode = navigator.SelectSingleNode("//tem:ValidateAdminUserResponse", nsManager);
                if (responseNode == null)
                {
                    // Try without namespace
                    responseNode = navigator.SelectSingleNode("//ValidateAdminUserResponse");
                }

                if (responseNode == null)
                {
                    _logger.LogError("Invalid SOAP response format for user {UserId}", userId);
                    return new AuthResult
                    {
                        Success = false,
                        Message = "Invalid response format from authentication service"
                    };
                }

                // Navigate to the response content
                var responseNavigator = responseNode.CreateNavigator();

                // Extract CODE
                var codeNode = responseNavigator.SelectSingleNode(".//CODE");
                if (codeNode == null)
                {
                    // Try case-insensitive
                    codeNode = responseNavigator.SelectSingleNode(".//*[local-name()='CODE']");
                }

                if (codeNode?.Value != "1000")
                {
                    var errorNode = responseNavigator.SelectSingleNode(".//ERROR");
                    if (errorNode == null)
                    {
                        errorNode = responseNavigator.SelectSingleNode(".//*[local-name()='ERROR']");
                    }

                    var errorMessage = errorNode?.Value ?? "Authentication failed";
                    _logger.LogWarning("Authentication failed for user {UserId}: {ErrorMessage}", userId, errorMessage);
                    return new AuthResult
                    {
                        Success = false,
                        Message = errorMessage
                    };
                }

                // Extract user data
                var userNode = responseNavigator.SelectSingleNode(".//USER");
                if (userNode == null)
                {
                    userNode = responseNavigator.SelectSingleNode(".//*[local-name()='USER']");
                }

                var result = new AuthResult
                {
                    Success = true,
                    UserId = userId
                };

                if (userNode != null)
                {
                    var userNavigator = userNode.CreateNavigator();

                    var idNode = userNavigator.SelectSingleNode(".//ID");
                    result.UserId = idNode?.Value ?? userId;

                    var nameNode = userNavigator.SelectSingleNode(".//NAME");
                    result.UserName = nameNode?.Value ?? userId;

                    var branchNode = userNavigator.SelectSingleNode(".//BRANCH");
                    result.BranchCode = branchNode?.Value;

                    var emailNode = userNavigator.SelectSingleNode(".//EMAIL");
                    result.Email = emailNode?.Value;
                }

                // Extract role
                var roleNode = responseNavigator.SelectSingleNode(".//ROLE");
                if (roleNode == null)
                {
                    roleNode = responseNavigator.SelectSingleNode(".//*[local-name()='ROLE']");
                }

                if (roleNode != null)
                {
                    var roleNavigator = roleNode.CreateNavigator();
                    var ridNode = roleNavigator.SelectSingleNode(".//RID");
                    result.RoleId = ridNode?.Value;
                }

                result.RawResponse = soapResponse;
                _logger.LogInformation("User {UserId} authenticated successfully", userId);

                return result;
            }
            catch (XmlException ex)
            {
                _logger.LogError(ex, "XML parsing error for user {UserId}", userId);
                return new AuthResult
                {
                    Success = false,
                    Message = $"Invalid response format: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing SOAP response for user {UserId}", userId);
                return new AuthResult
                {
                    Success = false,
                    Message = $"Error parsing authentication response: {ex.Message}"
                };
            }
        }
    }
}
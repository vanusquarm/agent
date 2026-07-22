using System;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Web;
using Statement.Web.Models;
using Statement.Web.Interfaces;

namespace Statement.Web.Services
{
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
                    <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
                                   xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
                                   xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                        <soap:Body>
                            <ValidateAdminUser xmlns=""http://tempuri.org/"">
                                <userid>{uid}</userid>
                                <password>{upass}</password>
                                <appid>{appId}</appid>
                            </ValidateAdminUser>
                        </soap:Body>
                    </soap:Envelope>";

                _logger.LogDebug("Sending SOAP request for user: {UserId}", uid);

                // Create HTTP request
                var httpRequest = (HttpWebRequest)WebRequest.Create(_soapEndpointUrl);
                httpRequest.Method = "POST";
                httpRequest.ContentType = "text/xml; charset=utf-8";
                httpRequest.Headers.Add("SOAPAction", "http://tempuri.org/ValidateAdminUser");
                httpRequest.Timeout = 30000; // 30 seconds

                // Write request body
                using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
                {
                    await streamWriter.WriteAsync(soapRequest);
                }

                // Get response
                using (var httpResponse = (HttpWebResponse)await httpRequest.GetResponseAsync())
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    string soapResponse = await streamReader.ReadToEndAsync();
                    _logger.LogDebug("SOAP response received for user: {UserId}", uid);

                    return ParseSoapResponse(soapResponse, uid);
                }
            }
            catch (WebException ex)
            {
                _logger.LogError(ex, "Web exception for {UserId}: {Message}", uid, ex.Message);

                if (ex.Response != null)
                {
                    using (var responseStream = ex.Response.GetResponseStream())
                    using (var reader = new StreamReader(responseStream))
                    {
                        var responseBody = await reader.ReadToEndAsync();
                        _logger.LogError("SOAP Error Response: {Response}", responseBody);
                        return new AuthResult
                        {
                            Success = false,
                            Message = $"Authentication service error"
                        };
                    }
                }

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
                // Load the SOAP response
                var document = new XmlDocument();
                document.LoadXml(soapResponse);

                var navigator = document.CreateNavigator();

                // Handle namespaces
                var nsManager = new XmlNamespaceManager(document.NameTable);
                nsManager.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
                nsManager.AddNamespace("tem", "http://tempuri.org/");

                // Get the result node
                var resultNode = navigator.SelectSingleNode("//tem:ValidateAdminUserResult", nsManager);
                if (resultNode == null)
                {
                    resultNode = navigator.SelectSingleNode("//ValidateAdminUserResult");
                }

                if (resultNode == null)
                {
                    _logger.LogError("No result node found in SOAP response for user {UserId}", userId);
                    return new AuthResult
                    {
                        Success = false,
                        Message = "Invalid response format from authentication service"
                    };
                }

                // IMPORTANT: The inner XML is HTML-encoded, so we need to decode it
                var innerXml = resultNode.InnerXml;

                // Decode HTML entities (&lt; -> <, &gt; -> >, etc.)
                var decodedXml = System.Net.WebUtility.HtmlDecode(innerXml);

                _logger.LogDebug("Decoded XML: {DecodedXml}", decodedXml);

                // Now parse the decoded XML
                var innerDocument = new XmlDocument();
                innerDocument.LoadXml(decodedXml);

                var innerNavigator = innerDocument.CreateNavigator();

                // Extract CODE
                var codeNode = innerNavigator.SelectSingleNode("/Response/CODE");
                if (codeNode == null)
                {
                    codeNode = innerNavigator.SelectSingleNode("/response/code");
                }

                if (codeNode?.Value != "1000")
                {
                    var errorNode = innerNavigator.SelectSingleNode("/Response/Error")
                                  ?? innerNavigator.SelectSingleNode("/Response/ERROR");

                    var errorMessage = errorNode?.Value ?? "Authentication failed";
                    _logger.LogWarning("Authentication failed for user {UserId}: {ErrorMessage} (Code: {Code})",
                        userId, errorMessage, codeNode?.Value);

                    return new AuthResult
                    {
                        Success = false,
                        Message = errorMessage,
                        RawResponse = soapResponse
                    };
                }

                // Extract user data
                var userNode = innerNavigator.SelectSingleNode("/Response/USER");
                var result = new AuthResult
                {
                    Success = true,
                    UserId = userId,
                    RawResponse = soapResponse
                };

                if (userNode != null)
                {
                    var userNavigator = userNode.CreateNavigator();

                    var idNode = userNavigator.SelectSingleNode("ID");
                    if (idNode != null) result.UserId = idNode.Value;

                    var nameNode = userNavigator.SelectSingleNode("NAME");
                    if (nameNode != null) result.UserName = nameNode.Value;

                    var branchNode = userNavigator.SelectSingleNode("BRANCH");
                    if (branchNode != null) result.BranchCode = branchNode.Value;

                    var emailNode = userNavigator.SelectSingleNode("EMAIL");
                    if (emailNode != null) result.Email = emailNode.Value;
                }

                // Extract role data
                var roleNode = innerNavigator.SelectSingleNode("/Response/ROLE");
                if (roleNode != null)
                {
                    var roleNavigator = roleNode.CreateNavigator();
                    var ridNode = roleNavigator.SelectSingleNode("RID");
                    if (ridNode != null) result.RoleId = ridNode.Value;
                }

                _logger.LogInformation("User {UserId} authenticated successfully", userId);
                return result;
            }
            catch (XmlException ex)
            {
                _logger.LogError(ex, "XML parsing error for user {UserId}: {Message}", userId, ex.Message);
                _logger.LogError("SOAP Response that caused error: {Response}", soapResponse);
                return new AuthResult
                {
                    Success = false,
                    Message = $"Invalid response format: {ex.Message}",
                    RawResponse = soapResponse
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing SOAP response for user {UserId}: {Message}", userId, ex.Message);
                return new AuthResult
                {
                    Success = false,
                    Message = $"Error parsing authentication response: {ex.Message}",
                    RawResponse = soapResponse
                };
            }
        }
    }
}
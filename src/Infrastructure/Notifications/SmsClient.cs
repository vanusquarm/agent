using Microsoft.Extensions.Configuration;
using RestSharp;
using RestSharp.Authenticators;

namespace Finacle.Infrastructure.Notifications;

public class SmsClient
{

    private readonly RestClient _client;

    public SmsClient(IConfiguration configuration)
    {
        var options = new RestClientOptions(configuration["Sms:Url"])
        {
            Authenticator = new HttpBasicAuthenticator(configuration["Sms:Username"], configuration["Sms:Password"])
        };
        _client = new RestClient(options);

    }

    public async Task<RestResponse> SendSms(string to, string content)
    {
        var body = new
        {
            From = "GTBANK",
            To = "+" + to,
            Content = content,
            RegisteredDelivery = true,
            Direction = "out"
        };
        var request = new RestRequest();
        request.AddJsonBody(body);
        return await _client.ExecutePostAsync(request);
    }
}
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Finacle.Core
{
    public partial class FinacleClient : IDisposable
    {
        protected readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public FinacleClient(string baseUrl, string channelId)
        {
            _baseUrl = baseUrl ?? throw new ArgumentException("Base URL cannot be null or empty.", nameof(baseUrl));
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_baseUrl)
            };
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("ChannelId", channelId.ToUpper());
        }

        public FinacleClient(HttpClient httpClient, string channelId = null)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
            if (!string.IsNullOrEmpty(channelId)) _httpClient.DefaultRequestHeaders.Add("ChannelId", channelId.ToUpper());
        }

        public void SetAccessToken(string accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
                throw new ArgumentException("Access token cannot be null or empty.", nameof(accessToken));

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        public void SetRequestHeader(string headerName, string headerValue)
        {
            if (string.IsNullOrWhiteSpace(headerName))
                throw new ArgumentException("Header name cannot be null or empty.", nameof(headerName));

            if (string.IsNullOrWhiteSpace(headerValue))
                throw new ArgumentException("Header value cannot be null or empty.", nameof(headerValue));

            _httpClient.DefaultRequestHeaders.Remove(headerName);
            _httpClient.DefaultRequestHeaders.Add(headerName, headerValue);
        }

        public static bool TryDeserialize<T>(string json, out T result)
        {
            try
            {
                result = JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return result != null;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"{nameof(T)} JSON Deserialization failed: {ex.Message}");
                result = default;
                return false;
            }
        }

        public static T TryDeserialize<T>(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            }
            catch (JsonException ex)
            {
                Console.WriteLine($"{nameof(T)} JSON Deserialization failed: {ex.Message}");
                return default;
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}

using Finacle.Infrastructure.Auth.OpenId;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Finacle.Infrastructure.Auth.BasicJwt
{
    public interface IJwtService
    {
        string GenerateToken();
        Task<string> GetTokenAsync();
    }

    public class JwtService : IJwtService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JwtOptions _options;
        private readonly IJwtStorage _tokenStorage;

        public JwtService(IHttpClientFactory httpClientFactory, IOptions<JwtOptions> options, IJwtStorage tokenStorage)
        {
            _httpClientFactory = httpClientFactory;
            _options = options.Value;
            _tokenStorage = tokenStorage;
        }

        // Generate local token
        public string GenerateToken()
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _options.Subject),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("scope", "api.read") // Custom claim for example
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _options.IssuerUrl,
                audience: _options.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(5), // Short expiry for demo purposes
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Validate locally generated token from request
        public bool ValidateToken(string token)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    IssuerSigningKey = key,
                    ValidateIssuer = false, // You can enable issuer validation if needed
                    ValidateAudience = false, // You can enable audience validation if needed
                    ValidateLifetime = true, // Validate token expiration
                    ClockSkew = TimeSpan.Zero // Optional: Adjust for skew in expiration time
                }, out var validatedToken);

                // You can access the claims in principal and authorize based on them
                var username = principal.Identity.Name; // Example: Get the username from the token
                return true; // Token is valid
            }
            catch (Exception)
            {
                return false; // Token is invalid
            }
        }

        // Call the API to get the token
        public async Task<string> GetTokenAsync()
        {
            if (string.IsNullOrEmpty(_tokenStorage.AccessToken) || DateTime.UtcNow >= _tokenStorage.ExpiresAt)
            {
                await RefreshTokenAsync(); // Retrieve a new token if it doesn't exist or is expired
            }

            return _tokenStorage.AccessToken;
        }

        // Refresh the token
        public async Task RefreshTokenAsync()
        {
            var client = _httpClientFactory.CreateClient();

            // Finacle authentication server's token endpoint
            var tokenEndpoint = _options.IssuerUrl;

            var formData = new Dictionary<string, string>
            {
                { "client_id", _options.ClientId },
                { "client_secret", _options.ClientSecret },
                { "grant_type", "client_credentials" }
            };

            // Create FormUrlEncodedContent from the form data
            var requestBody = new FormUrlEncodedContent(formData);

            // Send the POST request
            var response = await client.PostAsync(tokenEndpoint, requestBody);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error refreshing token.");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);

            _tokenStorage.AccessToken = tokenResponse?.AccessToken;
            _tokenStorage.RefreshToken = tokenResponse?.RefreshToken;  // Optional, depending on your OAuth flow
            _tokenStorage.ExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse?.ExpiresIn ?? 3600); // Set the expiration time
        }

        private class TokenResponse
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; }

            [JsonPropertyName("refresh_token")]
            public string RefreshToken { get; set; }

            [JsonPropertyName("expires_in")]
            public int ExpiresIn { get; set; }
        }
    }
}

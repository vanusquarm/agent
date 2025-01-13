using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finacle.Infrastructure.Auth.BasicJwt
{
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    public interface IJwtService
    {
        string GenerateToken();
    }

    public class JwtService : IJwtService
    {
        private const string SecretKey = "your_secret_key_here"; // Secret key for signing the JWT

        public string GenerateToken()
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, "dynamicUser"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("scope", "api.read") // Custom claim for example
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "your_service",
                audience: "your_audience",
                claims: claims,
                expires: DateTime.Now.AddMinutes(5), // Short expiry for demo purposes
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool ValidateToken(string token)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secret_key_here"));
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
    }

}

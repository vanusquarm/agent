using Microsoft.Extensions.Caching.Memory;

namespace Finacle.Infrastructure.Auth.BasicJwt;
public interface IJwtStorage
{
    string AccessToken { get; set; }
    string RefreshToken { get; set; }
    DateTime ExpiresAt { get; set; }
}

public class JwtStorage : IJwtStorage
{
    private readonly IMemoryCache _cache;
    private const string AccessTokenCacheKey = "AccessToken";
    private const string RefreshTokenCacheKey = "RefreshToken";
    private const string ExpiresAtCacheKey = "ExpiresAt";

    public JwtStorage(IMemoryCache cache)
    {
        _cache = cache;
    }

    public string AccessToken
    {
        get => _cache.TryGetValue(AccessTokenCacheKey, out string accessToken) ? accessToken : null;
        set => _cache.Set(AccessTokenCacheKey, value);
    }

    public string RefreshToken
    {
        get => _cache.TryGetValue(RefreshTokenCacheKey, out string refreshToken) ? refreshToken : null;
        set => _cache.Set(RefreshTokenCacheKey, value);
    }

    public DateTime ExpiresAt
    {
        get => _cache.TryGetValue(ExpiresAtCacheKey, out DateTime expiresAt) ? expiresAt : DateTime.MinValue;
        set => _cache.Set(ExpiresAtCacheKey, value);
    }
}

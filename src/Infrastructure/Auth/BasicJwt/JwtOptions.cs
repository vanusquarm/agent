
using System.ComponentModel.DataAnnotations;

namespace Finacle.Infrastructure.Auth.OpenId;
public class JwtOptions
{
    [Required(AllowEmptyStrings = false)]
    public string? Authority { get; set; } = string.Empty;
    [Required(AllowEmptyStrings = false)]
    public string? Audience { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false)]
    public string? ClientId { get; set; } = string.Empty;
    [Required(AllowEmptyStrings = false)]
    public string? ClientSecret { get; set; } = string.Empty;
    [Required(AllowEmptyStrings = false)]
    public string? IssuerUrl { get; set; } = string.Empty;
    [Required(AllowEmptyStrings = false)]
    public string? SecretKey { get; set; } = string.Empty;
    [Required(AllowEmptyStrings = false)]
    public string? AccessToken { get; set; } = string.Empty;
    [Required(AllowEmptyStrings = false)]
    public string? Subject { get; set; } = string.Empty;
}

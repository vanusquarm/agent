
using System.ComponentModel.DataAnnotations;

namespace Finacle.Infrastructure.Auth.OpenId;
public class OpenIdOptions
{
    [Required(AllowEmptyStrings = false)]
    public string? Authority { get; set; } = string.Empty;
    [Required(AllowEmptyStrings = false)]
    public string? Audience { get; set; } = string.Empty;
}

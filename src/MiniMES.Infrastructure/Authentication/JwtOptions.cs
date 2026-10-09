using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MiniMES.Infrastructure.Authentication;

public sealed class JwtOptions : IValidatableObject
{
    public const string SectionName = "Jwt";
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public int ExpireMinutes { get; set; }
    public int RefreshTokenExpireDays { get; set; } = 7;
    public int ClockSkewSeconds { get; set; } = 30;
    public string RefreshTokenDelivery { get; set; } = "Body";
    public string RefreshTokenCookieName { get; set; } = "mini_mes_refresh_token";

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Issuer)) yield return new ValidationResult("Jwt:Issuer is required.");
        if (string.IsNullOrWhiteSpace(Audience)) yield return new ValidationResult("Jwt:Audience is required.");
        if (string.IsNullOrWhiteSpace(Key) || Encoding.UTF8.GetByteCount(Key) < 32) yield return new ValidationResult("Jwt:Key must be at least 32 bytes.");
        if (ExpireMinutes is < 1 or > 1440) yield return new ValidationResult("Jwt:ExpireMinutes must be between 1 and 1440.");
        if (RefreshTokenExpireDays is < 1 or > 90) yield return new ValidationResult("Jwt:RefreshTokenExpireDays must be between 1 and 90.");
        if (ClockSkewSeconds is < 0 or > 300) yield return new ValidationResult("Jwt:ClockSkewSeconds must be between 0 and 300.");
        if (!string.Equals(RefreshTokenDelivery, "Body", StringComparison.OrdinalIgnoreCase) && !string.Equals(RefreshTokenDelivery, "Cookie", StringComparison.OrdinalIgnoreCase)) yield return new ValidationResult("Jwt:RefreshTokenDelivery must be Body or Cookie.");
    }
}

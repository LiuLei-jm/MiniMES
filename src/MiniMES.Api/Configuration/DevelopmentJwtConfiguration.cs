using System.Security.Cryptography;
using MiniMES.Infrastructure.Authentication;

namespace MiniMES.Api.Configuration;

public static class DevelopmentJwtConfiguration
{
    public static void EnsureSigningKey(IConfiguration configuration)
    {
        var keyName = $"{JwtOptions.SectionName}:Key";

        if (!string.IsNullOrWhiteSpace(configuration[keyName]))
        {
            return;
        }

        configuration[keyName] = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    }
}

using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using MiniMES.Api.Configuration;
using MiniMES.Application.Features.Identity.Refresh;
using MiniMES.Infrastructure.Authentication;
using Xunit;

namespace MiniMES.ArchitectureTests;

public sealed class AuthenticationSecurityTests
{
    [Fact]
    public void RefreshToken_is_random_and_only_its_hash_is_persisted()
    {
        var service = new RefreshTokenService(Options.Create(BuildOptions()));
        var first = service.Create(Guid.NewGuid());
        var second = service.Create(Guid.NewGuid());
        Assert.NotEqual(first.Token, second.Token);
        Assert.Equal(64, first.Entity.TokenHash.Length);
        Assert.NotEqual(first.Token, first.Entity.TokenHash);
        Assert.Equal(first.Entity.TokenHash, service.Hash(first.Token));
    }

    [Fact]
    public async Task Refresh_command_rejects_empty_and_oversized_tokens()
    {
        var validator = new RefreshTokenCommandValidator();
        Assert.False((await validator.ValidateAsync(new RefreshTokenCommand(string.Empty))).IsValid);
        Assert.False((await validator.ValidateAsync(new RefreshTokenCommand(new string('x', 513)))).IsValid);
    }

    [Fact]
    public void Jwt_options_reject_insecure_key_and_invalid_expirations()
    {
        var invalid = new JwtOptions { Issuer = "issuer", Audience = "audience", Key = "short", ExpireMinutes = 0, RefreshTokenExpireDays = 0 };
        Assert.NotEmpty(invalid.Validate(new System.ComponentModel.DataAnnotations.ValidationContext(invalid)));
    }

    [Fact]
    public void Development_jwt_configuration_generates_a_key_without_overwriting_an_existing_one()
    {
        var configuration = new ConfigurationManager();

        DevelopmentJwtConfiguration.EnsureSigningKey(configuration);

        var generatedKey = configuration[$"{JwtOptions.SectionName}:Key"];
        Assert.NotNull(generatedKey);
        Assert.True(System.Text.Encoding.UTF8.GetByteCount(generatedKey) >= 32);

        DevelopmentJwtConfiguration.EnsureSigningKey(configuration);
        Assert.Equal(generatedKey, configuration[$"{JwtOptions.SectionName}:Key"]);
    }

    private static JwtOptions BuildOptions() => new() { Issuer = "issuer", Audience = "audience", Key = "01234567890123456789012345678901", ExpireMinutes = 60, RefreshTokenExpireDays = 7 };
}

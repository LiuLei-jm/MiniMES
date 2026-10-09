using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MiniMES.Api;
using Swashbuckle.AspNetCore.SwaggerGen;
using Xunit;

namespace MiniMES.ArchitectureTests;

public sealed class SwaggerSecurityTests
{
    [Fact]
    public void Swagger_defines_global_jwt_bearer_authentication()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Issuer"] = "test-issuer",
                ["Jwt:Audience"] = "test-audience",
                ["Jwt:Key"] = "01234567890123456789012345678901",
                ["Jwt:ExpireMinutes"] = "60"
            })
            .Build();
        var services = new ServiceCollection();

        services.AddApi(configuration);

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<SwaggerGenOptions>>().Value;
        var scheme = options.SwaggerGeneratorOptions.SecuritySchemes["Bearer"];

        Assert.Equal(SecuritySchemeType.Http, scheme.Type);
        Assert.Equal("bearer", scheme.Scheme);
        Assert.Equal("JWT", scheme.BearerFormat);
        Assert.NotEmpty(options.SwaggerGeneratorOptions.SecurityRequirements);
    }
}

using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MiniMES.Api;
using MiniMES.Api.Common.Middleware;
using MiniMES.Api.Configuration;
using Xunit;

namespace MiniMES.ArchitectureTests;

public sealed class CorsPolicyTests
{
    [Fact]
    public void Frontend_policy_allows_configured_origins_and_api_request_headers()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Issuer"] = "test-issuer",
                ["Jwt:Audience"] = "test-audience",
                ["Jwt:Key"] = "01234567890123456789012345678901",
                ["Jwt:ExpireMinutes"] = "60",
                [$"{CorsPolicyNames.AllowedOriginsSection}:0"] = "http://localhost:5173/"
            })
            .Build();
        var services = new ServiceCollection();

        services.AddApi(configuration);

        using var provider = services.BuildServiceProvider();
        var corsOptions = provider.GetRequiredService<IOptions<CorsOptions>>().Value;
        var policy = corsOptions.GetPolicy(CorsPolicyNames.Frontend);

        Assert.NotNull(policy);
        Assert.Contains("http://localhost:5173", policy.Origins);
        Assert.True(policy.AllowAnyHeader);
        Assert.True(policy.AllowAnyMethod);
        Assert.Contains(CorrelationIdMiddleware.HeaderName, policy.ExposedHeaders);
    }
}

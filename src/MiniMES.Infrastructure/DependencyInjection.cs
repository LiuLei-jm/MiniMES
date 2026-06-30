using MiniMES.Application.Common.Interfaces;
using MiniMES.Application.Common.Security;
using MiniMES.Infrastructure.Identity;
using MiniMES.Infrastructure.Persistence;
using MiniMES.Infrastructure.Persistence.Interceptors;
using MiniMES.Infrastructure.Security;

namespace MiniMES.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("Default"));
        });
        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton(provider =>
        {
            var config = provider.GetRequiredService<IConfiguration>();
            return new JwtOptions
            {
                Issuer = config["Jwt:Issuer"]!,
                Audience = config["Jwt:Audience"]!,
                Key = config["Jwt:Key"]!,
                ExpireMinutes = int.Parse(config["Jwt:ExpireMinutes"]!)
            };
        });
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<AppDbCommandInterceptor>();
        return services;
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MiniMES.Application.Common.Security;
using MiniMES.Domain.Interfaces;
using MiniMES.Infrastructure.Identity;
using MiniMES.Infrastructure.Persistence;
using MiniMES.Infrastructure.Repositories;
using MiniMES.Infrastructure.Security;
using Microsoft.AspNetCore.Http;

namespace MiniMES.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("Default"));
        });
        services.AddScoped<IUserRepository, UserRepository>();
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
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();
        return services;
    }
}
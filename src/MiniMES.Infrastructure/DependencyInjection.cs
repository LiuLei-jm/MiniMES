using Microsoft.AspNetCore.Authorization;
using MiniMES.Application.Authentication.Interfaces;
using MiniMES.Application.Common.Interfaces;
using MiniMES.Infrastructure.Authentication;
using MiniMES.Infrastructure.Authorization;
using MiniMES.Infrastructure.Persistence;
using MiniMES.Infrastructure.Persistence.Interceptors;

namespace MiniMES.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<AppDbCommandInterceptor>();
        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            options.UseSqlServer(configuration.GetConnectionString("Default"));
            options.AddInterceptors(sp.GetRequiredService<AppDbCommandInterceptor>());
        });
        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IUserPermissionService, UserPermissionService>();
        services.AddScoped<ILoginAttemptService, LoginAttemptService>();
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        return services;
    }
}
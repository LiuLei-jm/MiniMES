using Microsoft.AspNetCore.Authorization;
using MiniMES.Application.Authentication.Interfaces;
using MiniMES.Application.Common.Interfaces;
using MiniMES.Infrastructure.Authentication;
using MiniMES.Infrastructure.Authorization;
using MiniMES.Infrastructure.Persistence;
using MiniMES.Infrastructure.Persistence.Interceptors;
using System.ComponentModel.DataAnnotations;

namespace MiniMES.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDistributedMemoryCache();
        services.AddScoped<AppDbCommandInterceptor>();
        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            DatabaseProvider.Configure(options, configuration);
            options.AddInterceptors(sp.GetRequiredService<AppDbCommandInterceptor>());
        });
        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddOptions<JwtOptions>().Bind(configuration.GetSection(JwtOptions.SectionName)).Validate(options => Validator.TryValidateObject(options, new ValidationContext(options), null, true), "Invalid Jwt configuration.").ValidateOnStart();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IUserPermissionService, UserPermissionService>();
        services.AddScoped<ILoginAttemptService, LoginAttemptService>();
        services.AddScoped<ISecurityAuditService, SecurityAuditService>();
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        return services;
    }
}

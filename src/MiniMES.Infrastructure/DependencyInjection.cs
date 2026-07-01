using MiniMES.Application.Common.Interfaces;
using MiniMES.Application.Common.Interfaces.Security;
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
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IUserPermissionService, UserPermissionService>();
        return services;
    }
}
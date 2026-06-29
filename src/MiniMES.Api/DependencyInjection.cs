using MiniMES.Api.Common.Exceptions;

namespace MiniMES.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        var jwt = configuration.GetSection("Jwt");
        services.AddFastEndpoints()
            .AddSwaggerGen();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwt["Issuer"],
                    ValidAudience = jwt["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwt["Key"]!))
                };
            }
            );
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
            {
                policy.RequireRole("Admin");
            });
        });
        return services;
    }
}

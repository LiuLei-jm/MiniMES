using Microsoft.Extensions.DependencyInjection;
using MiniMES.Application.Common.Behaviors;

namespace MiniMES.Application;

public static class DependencyIndjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        return services;
    }
}

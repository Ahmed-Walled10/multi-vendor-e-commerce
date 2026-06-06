using System.Reflection;
using ECommerce.Application.Common.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // MediatR — registers all handlers in this assembly
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // AutoMapper — registers all profiles in this assembly
        services.AddAutoMapper(assembly);

        // FluentValidation — registers all validators in this assembly
        services.AddValidatorsFromAssembly(assembly);

        // Pipeline Behaviors (order matters: logging runs first, then validation)
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // Caching
        services.AddSingleton<Common.Caching.IProductCacheSignal, Common.Caching.ProductCacheSignal>();

        return services;
    }
}

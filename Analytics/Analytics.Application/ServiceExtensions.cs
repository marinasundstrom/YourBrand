using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using YourBrand.Analytics.Application.Behaviors;
using YourBrand.Analytics.Application.Hubs;
using System.Text.Json.Serialization;
using System.Text.Json;
using YourBrand.Analytics.Application.Features.Tracking;

namespace YourBrand.Analytics.Application;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(typeof(ServiceExtensions));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddValidatorsFromAssembly(typeof(ServiceExtensions).Assembly);

        return services;
    }

    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllersForApp();

        services.AddScoped<ITodoNotificationService, TodoNotificationService>();

        return services;
    }

    public static IServiceCollection AddControllersForApp(this IServiceCollection services)
    {
        var assembly = typeof(ClientController).Assembly;

        services.AddControllers()
            .AddApplicationPart(assembly)
            .AddJsonOptions(jsonOptions =>
            {
                jsonOptions.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            });

        return services;
    }
}

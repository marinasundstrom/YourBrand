using FluentValidation;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

using YourBrand.Meetings.Behaviors;

namespace YourBrand.Meetings;

public static class ServiceExtensions2
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining(typeof(ServiceExtensions)));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddValidatorsFromAssembly(typeof(ServiceExtensions).Assembly);

        /*
        services.AddScoped<IDtoFactory, DtoFactory>();
        services.AddScoped<IDtoComposer, DtoComposer>();
        */

        return services;
    }

    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllersForApp();

        //services.AddScoped<ITicketNotificationService, TicketNotificationService>();

        return services;
    }

    public static IServiceCollection AddControllersForApp(this IServiceCollection services)
    {
        var assembly = typeof(ServiceExtensions2).Assembly;

        services.AddControllers()
            .AddApplicationPart(assembly);

        return services;
    }
}
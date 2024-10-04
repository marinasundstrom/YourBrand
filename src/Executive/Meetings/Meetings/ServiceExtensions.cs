using MassTransit;

using YourBrand.Integration;
using YourBrand.Meetings;
using YourBrand.Meetings.Consumers;
using YourBrand.Meetings.Domain;
using YourBrand.Meetings.Infrastructure;

namespace YourBrand.Meetings;

public static class ServiceExtensions
{
    public static IServiceCollection AddUniverse(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddPresentation()
            .AddApplication()
            .AddInfrastructure(configuration);

        return services;
    }

    public static IServiceCollection AddMassTransitForApp(this IServiceCollection services)
    {
        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();

            x.AddConsumers(typeof(Program).Assembly);

            //x.AddConsumersForApp();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.UseTenancyFilters(context);
                cfg.UseIdentityFilters(context);

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
using System.Reflection;

using MassTransit;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace YourBrand.Extensions;

public static class ServiceBusExtensions
{
    public static IServiceCollection AddServiceBus(this IServiceCollection services, Action<IBusRegistrationConfigurator> conf)
    {
        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();

            //x.AddConsumers(Assembly.GetExecutingAssembly());

            conf?.Invoke(x);
        });

        return services;
    }

    public static void UsingAzureServiceBus(this IBusRegistrationConfigurator x, IConfiguration configuration)
    {
        x.UsingAzureServiceBus((context, cfg) =>
        {
            cfg.Host($"sb://{configuration["Azure:ServiceBus:Namespace"]}.servicebus.windows.net");

            cfg.ConfigureEndpoints(context);
        });
    }

    public static void UsingRabbitMQ(this IBusRegistrationConfigurator x, IConfiguration configuration)
    {
        x.UsingRabbitMq((context, cfg) =>
        {
            var rabbitmqHost = configuration["RABBITMQ_HOST"] ?? "localhost";

            cfg.Host(rabbitmqHost, "/", h =>
            {
                h.Username("guest");
                h.Password("guest");
            });

            cfg.ConfigureEndpoints(context);
        });
    }
}
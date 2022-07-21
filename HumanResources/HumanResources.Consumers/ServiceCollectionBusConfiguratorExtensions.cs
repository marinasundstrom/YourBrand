using System;

using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;

using YourBrand.HumanResources.Application.Common.Interfaces;
using YourBrand.Identity;

namespace YourBrand.HumanResources.Consumers;

public static class ServiceCollectionBusConfiguratorExtensions
{
    public static void AddAppConsumers(this IServiceCollectionBusConfigurator conf)
    {
        conf.AddConsumers(typeof(GetPersonConsumer).Assembly);
    }
}
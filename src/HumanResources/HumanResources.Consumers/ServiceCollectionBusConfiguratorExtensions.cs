using System;

using MassTransit;
using YourBrand.HumanResources.Application.Common.Interfaces;
using YourBrand.Identity;

namespace YourBrand.HumanResources.Consumers;

public static class ServiceCollectionBusConfiguratorExtensions
{
    public static void AddAppConsumers(this IBusRegistrationConfigurator conf)
    {
        conf.AddConsumers(typeof(GetPersonConsumer).Assembly);
    }
}
using System;

using MassTransit;
using YourBrand.UserManagement.Application.Common.Interfaces;
using YourBrand.Identity;

namespace YourBrand.UserManagement.Consumers;

public static class ServiceCollectionBusConfiguratorExtensions
{
    public static void AddAppConsumers(this IBusRegistrationConfigurator conf)
    {
        conf.AddConsumers(typeof(GetUserConsumer).Assembly);
    }
}
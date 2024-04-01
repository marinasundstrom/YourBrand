using MassTransit;

namespace YourBrand.HumanResources.Consumers;

public static class ServiceCollectionBusConfiguratorExtensions
{
    public static void AddAppConsumers(this IBusRegistrationConfigurator conf)
    {
        conf.AddConsumers(typeof(GetPersonConsumer).Assembly);
    }
}
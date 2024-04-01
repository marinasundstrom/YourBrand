using MassTransit;

namespace YourBrand.IdentityManagement.Consumers;

public static class ServiceCollectionBusConfiguratorExtensions
{
    public static void AddAppConsumers(this IBusRegistrationConfigurator conf)
    {
        conf.AddConsumers(typeof(GetUserConsumer).Assembly);
    }
}
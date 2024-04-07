namespace YourBrand.Integration;

using MassTransit;
using MassTransit.Configuration;

public static class TenancyFilterExtensions
{
    public static void UseTenancyFilters(this IPipeConfigurator<ConsumeContext> configurator, IRegistrationContext context)
    {
        ((IPublishPipelineConfigurator)configurator).UsePublishFilter(typeof(AddTenantIdPublishFilter<>), context);
        ((ISendPipelineConfigurator)configurator).UseSendFilter(typeof(AddTenantIdSendFilter<>), context);
        ((IConsumePipeConfigurator)configurator).UseConsumeFilter(typeof(ReadTenantIdConsumeFilter<>), context);
    }
}
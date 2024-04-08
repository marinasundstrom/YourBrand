namespace YourBrand.Integration;

using MassTransit;
using MassTransit.Configuration;

public static class IdentityFilterExtensions
{
    public static void UseIdentityFilters(this IPipeConfigurator<ConsumeContext> configurator, IRegistrationContext context)
    {
        ((IPublishPipelineConfigurator)configurator).UsePublishFilter(typeof(AddUserIdPublishFilter<>), context);
        ((ISendPipelineConfigurator)configurator).UseSendFilter(typeof(AddUserIdSendFilter<>), context);
        ((IConsumePipeConfigurator)configurator).UseConsumeFilter(typeof(ReadUserIdConsumeFilter<>), context);
    }
}
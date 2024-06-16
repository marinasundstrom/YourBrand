using MassTransit;

namespace YourBrand.ChatApp.Features.Chat;

public static class MassTransitExtensions
{
    public static IBusRegistrationConfigurator AddMessageConsumers(this IBusRegistrationConfigurator busRegistrationConfigurator)
    {
        //busRegistrationConfigurator.AddConsumer<UpdateStatusConsumer>();

        return busRegistrationConfigurator;
    }
}
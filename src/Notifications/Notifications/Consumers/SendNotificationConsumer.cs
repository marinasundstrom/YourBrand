using MassTransit;

using MediatR;

using YourBrand.Identity;
using YourBrand.Notifications.Application;
using YourBrand.Notifications.Application.Notifications.Commands;
using YourBrand.Notifications.Contracts;
using YourBrand.Tenancy;

namespace YourBrand.Notifications.Consumers;

public class SendNotificationConsumer(IMediator mediator) : IConsumer<SendNotification>
{
    public async Task Consume(ConsumeContext<SendNotification> context)
    {
        var message = context.Message;

        await mediator.Send(new CreateNotificationCommand(message.Content, message.Link, message.UserId, message.ScheduledFor));

        await context.RespondAsync(new SendNotificationResponse());
    }
}

public class MarkAllNotificationsAsReadConsumer(IMediator mediator) : IConsumer<MarkAllNotificationsAsRead>
{
    public async Task Consume(ConsumeContext<MarkAllNotificationsAsRead> context)
    {
        var message = context.Message;

        await mediator.Send(new MarkAllNotificationsAsReadCommand());

        await context.RespondAsync(new MarkAllNotificationsAsReadResponse());
    }
}

public class MarkNotificationAsReadConsumer(IMediator mediator) : IConsumer<MarkNotificationAsRead>
{
    public async Task Consume(ConsumeContext<MarkNotificationAsRead> context)
    {
        var message = context.Message;

        await mediator.Send(new MarkNotificationAsReadCommand(message.NotificationId));

        await context.RespondAsync(new MarkNotificationAsReadResponse());
    }
}
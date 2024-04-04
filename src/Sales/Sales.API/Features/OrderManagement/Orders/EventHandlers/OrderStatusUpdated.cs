using YourBrand.Domain;
using YourBrand.Notifications.Client;
using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Domain.Events;
using YourBrand.Sales.Features.OrderManagement.Repositories;

namespace YourBrand.Sales.Features.OrderManagement.Orders.EventHandlers;

public sealed class OrderStatusUpdatedEventHandler(IOrderRepository orderRepository,
    ICurrentUserService currentUserService,
    IEmailService emailService,
    IOrderNotificationService orderNotificationService,
    INotificationsClient notificationsClient,
    ILogger<OrderStatusUpdatedEventHandler> logger) : IDomainEventHandler<OrderStatusUpdated>
{
    private readonly IOrderRepository orderRepository = orderRepository;
    private readonly ICurrentUserService currentUserService = currentUserService;
    private readonly IEmailService emailService = emailService;
    private readonly IOrderNotificationService orderNotificationService = orderNotificationService;
    private readonly INotificationsClient _notificationsClient = notificationsClient;

    public async Task Handle(OrderStatusUpdated notification, CancellationToken cancellationToken)
    {
        var order = await orderRepository.FindByIdAsync(notification.OrderId, cancellationToken);

        if (order is null)
            return;

        await orderNotificationService.StatusUpdated(order.OrderNo, order.Status.ToDto());

        if (order.StatusId == 2)
        {
            await PostNotification(order);
        }

        if (order.AssigneeId is not null && order.LastModifiedById != order.AssigneeId)
        {/*
            await emailService.SendEmail(order.AssigneeId!.Email,
                $"Status of \"{order.Title}\" [{order.OrderNo}] changed to {notification.NewStatus}.",
                $"{order.LastModifiedBy!.Name} changed status of \"{order.Title}\" [{order.OrderNo}] from {notification.OldStatus} to {notification.NewStatus}."); */
        }
    }

    private async Task PostNotification(Order order)
    {
        try
        {
            await _notificationsClient.CreateNotificationAsync(new CreateNotification
            {
                Content = $"New order #{order.OrderNo}.",
                UserId = order.CreatedById,
                Link = $"/orders/{order.OrderNo}"
            });
        }
        catch (Exception exc)
        {
            logger.LogError(exc, "Failed to post notification.");
        }
    }
}
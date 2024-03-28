using MediatR;

using YourBrand.Sales.API.Features.OrderManagement.Domain.Events;
using YourBrand.Sales.API.Features.OrderManagement.Orders.Dtos;
using YourBrand.Sales.API.Features.OrderManagement.Repositories;

using YourBrand.Sales.Features.Common;
using YourBrand.Sales.Features.Services;
using YourBrand.Domain;
using YourBrand.Sales.API.Features.OrderManagement.Domain.Entities;
using YourBrand.Notifications.Client;

namespace YourBrand.Sales.API.Features.OrderManagement.Orders.EventHandlers;

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

        if(order.StatusId == 2) 
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
            await _notificationsClient.CreateNotificationAsync(new CreateNotificationDto
            {
                Title = "Sales",
                Text = $"New order #{order.OrderNo}.",
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
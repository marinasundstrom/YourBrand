using System;

using MediatR;

using YourBrand.Sales.API.Features.OrderManagement.Domain.Events;
using YourBrand.Sales.API.Features.OrderManagement.Repositories;

using YourBrand.Orders.Application.Common;
using YourBrand.Orders.Application.Services;
using YourBrand.Domain;
using YourBrand.Notifications.Client;
using YourBrand.Sales.API.Features.OrderManagement.Domain.Entities;

namespace YourBrand.Sales.API.Features.OrderManagement.Orders.EventHandlers;

public sealed class OrderCreatedEventHandler(IOrderRepository orderRepository, INotificationsClient notificationsClient,
    ILogger<OrderCreatedEventHandler> logger, ICurrentUserService currentUserService) : IDomainEventHandler<OrderCreated>
{
    private readonly IOrderRepository orderRepository = orderRepository;

    public async Task Handle(OrderCreated notification, CancellationToken cancellationToken)
    {
        var order = await orderRepository.FindByIdAsync(notification.OrderId, cancellationToken);

        if (order is null)
            return;

        Console.WriteLine("CREATED C");

        if (order.StatusId == 2)
        {
            await PostNotification(order);
        }
    }

    private async Task PostNotification(Order order)
    {
        try
        {
            await notificationsClient.CreateNotificationAsync(new CreateNotificationDto
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
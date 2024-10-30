using System;

using Azure.Core;

using Microsoft.EntityFrameworkCore;

using YourBrand.Domain;
using YourBrand.Notifications;
using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Domain.Events;
using YourBrand.Sales.Features.OrderManagement.Repositories;
using YourBrand.Sales.Persistence;
using YourBrand.Sales.Persistence.Repositories.Mocks;

namespace YourBrand.Sales.Features.OrderManagement.Orders.EventHandlers;

public sealed class OrderStatusUpdatedEventHandler(
    SalesContext salesContext,
    IOrderNotificationService orderNotificationService,
    INotificationService notificationsService,
    TimeProvider timeProvider,
    OrderNumberFetcher orderNumberFetcher,
    ILogger<OrderStatusUpdatedEventHandler> logger) : IDomainEventHandler<OrderStatusUpdated>
{
    private readonly IOrderNotificationService orderNotificationService = orderNotificationService;
    private readonly INotificationService _notificationService = notificationsService;

    public async Task Handle(OrderStatusUpdated notification, CancellationToken cancellationToken)
    {
        var order = await salesContext.Orders
            .IncludeAll()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == notification.OrderId, cancellationToken);

        if (order is null)
            return;

        await orderNotificationService.StatusUpdated(order.OrderNo.GetValueOrDefault(), order.Status.ToDto());

        if (notification.OldStatus == (int)OrderStatusEnum.Draft && order.OrderNo is null)
        {
            logger.LogInformation("Attempting to assign No to Order");

            await order.AssignOrderNo(orderNumberFetcher, cancellationToken);

            await salesContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Order number was assigned");
        }

       /* if (order.StatusId == (int)OrderStatusEnum.Confirmed)
        {
            if (order.Subscription is not null)
            {
                logger.LogInformation("Attempting to activate subscription");

                await ActivateSubscription(order, cancellationToken);
            }

            await PostNotification(order, cancellationToken);
        } */

        if (order.AssigneeId is not null && order.LastModifiedById != order.AssigneeId)
        {/*
            await emailService.SendEmail(order.AssigneeId!.Email,
                $"Status of \"{order.Title}\" [{order.OrderNo}] changed to {notification.NewStatus}.",
                $"{order.LastModifiedBy!.Name} changed status of \"{order.Title}\" [{order.OrderNo}] from {notification.OldStatus} to {notification.NewStatus}."); */
        }
    }

    private async Task ActivateSubscription(Order order, CancellationToken cancellationToken)
    {
        if (order.Subscription is null)
            return;

        var subscription = order.Subscription;
        var subscriptionPlan = subscription.Plan;

        //subscription.StartTrial(subscriptionPlan.TrialLength, timeProvider);
        subscription.Activate(timeProvider);

        await salesContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Subscription activated");
    }

    private async Task PostNotification(Order order, CancellationToken cancellationToken)
    {
        try
        {
            await _notificationService.PublishNotificationAsync(new Notification($"New order #{order.OrderNo}.")
            {
                UserId = order.CreatedById,
                Link = $"/orders/{order.OrderNo}"
            }, cancellationToken);

            logger.LogInformation("Posted \"New order\" notification.");
        }
        catch (Exception exc)
        {
            logger.LogError(exc, "Failed to post notification.");
        }
    }
}
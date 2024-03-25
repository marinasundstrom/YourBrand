using MediatR;

using YourBrand.Sales.API.Features.OrderManagement.Domain.Events;
using YourBrand.Sales.API.Features.OrderManagement.Repositories;

using YourBrand.Sales.Features.Common;
using YourBrand.Sales.Features.Services;
using YourBrand.Domain;

namespace YourBrand.Sales.API.Features.OrderManagement.Orders.EventHandlers;

public sealed class OrderUpdatedEventHandler(IOrderRepository orderRepository, IOrderNotificationService orderNotificationService) : IDomainEventHandler<OrderUpdated>
{
    private readonly IOrderRepository orderRepository = orderRepository;
    private readonly IOrderNotificationService orderNotificationService = orderNotificationService;

    public async Task Handle(OrderUpdated notification, CancellationToken cancellationToken)
    {
        var order = await orderRepository.FindByIdAsync(notification.OrderId, cancellationToken);

        if (order is null)
            return;

        //await orderNotificationService.Updated(order.OrderNo);
    }
}
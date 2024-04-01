using YourBrand.Domain;
using YourBrand.Sales.Domain.Events;
using YourBrand.Sales.Features.OrderManagement.Repositories;

namespace YourBrand.Sales.Features.OrderManagement.Orders.EventHandlers;

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
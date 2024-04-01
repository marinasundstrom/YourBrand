using YourBrand.Domain;
using YourBrand.Sales.Domain.Events;
using YourBrand.Sales.Features.OrderManagement.Repositories;

namespace YourBrand.Sales.Features.OrderManagement.Orders.EventHandlers;

public sealed class OrderDeletedEventHandler(IOrderRepository orderRepository, IOrderNotificationService orderNotificationService) : IDomainEventHandler<OrderDeleted>
{
    private readonly IOrderRepository orderRepository = orderRepository;
    private readonly IOrderNotificationService orderNotificationService = orderNotificationService;

    public async Task Handle(OrderDeleted notification, CancellationToken cancellationToken)
    {
        await orderNotificationService.Deleted(notification.OrderNo);
    }
}
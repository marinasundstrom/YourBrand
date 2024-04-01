using YourBrand.Sales.Domain.Events;
using YourBrand.Sales.Features.OrderManagement.Repositories;

using YourBrand.Sales;
using YourBrand.Sales.Services;
using YourBrand.Domain;

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
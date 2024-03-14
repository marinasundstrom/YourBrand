using YourBrand.Sales.API.Features.OrderManagement.Domain.Events;
using YourBrand.Sales.API.Features.OrderManagement.Repositories;

using YourBrand.Orders.Application.Common;
using YourBrand.Orders.Application.Services;
using YourBrand.Domain;

namespace YourBrand.Sales.API.Features.OrderManagement.Orders.EventHandlers;

public sealed class OrderDeletedEventHandler(IOrderRepository orderRepository, IOrderNotificationService orderNotificationService) : IDomainEventHandler<OrderDeleted>
{
    private readonly IOrderRepository orderRepository = orderRepository;
    private readonly IOrderNotificationService orderNotificationService = orderNotificationService;

    public async Task Handle(OrderDeleted notification, CancellationToken cancellationToken)
    {
        await orderNotificationService.Deleted(notification.OrderNo);
    }
}
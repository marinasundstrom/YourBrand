using YourBrand.Sales.Domain.Events;
using YourBrand.Sales.Features.OrderManagement.Repositories;

using YourBrand.Sales;
using YourBrand.Sales.Services;
using YourBrand.Domain;

namespace YourBrand.Sales.Features.OrderManagement.Orders.EventHandlers;

public sealed class OrderAssignedUserEventHandler(IOrderRepository orderRepository, IEmailService emailService, IOrderNotificationService orderNotificationService) : IDomainEventHandler<OrderAssignedUserUpdated>
{
    private readonly IOrderRepository orderRepository = orderRepository;
    private readonly IEmailService emailService = emailService;
    private readonly IOrderNotificationService orderNotificationService = orderNotificationService;

    public async Task Handle(OrderAssignedUserUpdated notification, CancellationToken cancellationToken)
    {
        var order = await orderRepository.FindByIdAsync(notification.OrderId, cancellationToken);

        if (order is null)
            return;

        if (order.AssigneeId is not null && order.LastModifiedById != order.AssigneeId)
        {
            /*
            await emailService.SendEmail(
                order.AssigneeId!.Email,
                $"You were assigned to \"{order.Title}\" [{order.OrderNo}].",
                $"{order.LastModifiedBy!.Name} assigned {order.AssigneeId.Name} to \"{order.Title}\" [{order.OrderNo}]."); */
        }
    }
}
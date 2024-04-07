using MediatR;

using YourBrand.Sales.Features.OrderManagement.Orders.Dtos;
using YourBrand.Sales.Features.OrderManagement.Repositories;

namespace YourBrand.Sales.Features.OrderManagement.Orders.Items;

public sealed record GetOrderItemById(string OrganizationId, string OrderId, string OrderItemId) : IRequest<Result<OrderItemDto>>
{
    public sealed class Handler(IOrderRepository orderRepository, IUnitOfWork unitOfWork) : IRequestHandler<GetOrderItemById, Result<OrderItemDto>>
    {
        public async Task<Result<OrderItemDto>> Handle(GetOrderItemById request, CancellationToken cancellationToken)
        {
            var order = await orderRepository.FindByIdAsync(request.OrderId, cancellationToken);

            if (order is null)
            {
                return Errors.Orders.OrderNotFound;
            }

            var orderItem = order.Items.FirstOrDefault(x => x.Id == request.OrderItemId);

            if (orderItem is null)
            {
                return Errors.Orders.OrderItemNotFound;
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return orderItem.ToDto();
        }
    }
}
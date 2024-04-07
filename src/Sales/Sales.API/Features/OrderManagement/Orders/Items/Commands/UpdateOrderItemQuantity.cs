using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Features.OrderManagement.Orders.Dtos;
using YourBrand.Sales.Features.OrderManagement.Repositories;

namespace YourBrand.Sales.Features.OrderManagement.Orders.Items.Commands;

public sealed record UpdateOrderItemQuantity(string OrganizationId, string OrderId, string OrderItemId, double Quantity) : IRequest<Result<OrderItemDto>>
{
    public sealed class Validator : AbstractValidator<UpdateOrderItemQuantity>
    {
        public Validator()
        {
            RuleFor(x => x.OrderId).NotNull().NotEmpty();

            RuleFor(x => x.Quantity);
        }
    }

    public sealed class Handler(IOrderRepository orderRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateOrderItemQuantity, Result<OrderItemDto>>
    {
        public async Task<Result<OrderItemDto>> Handle(UpdateOrderItemQuantity request, CancellationToken cancellationToken)
        {
            var order = await orderRepository
                                        .GetAll()
                                        .Where(x => x.OrganizationId == request.OrganizationId)
                                        .FirstOrDefaultAsync(x => x.Id == request.OrderId, cancellationToken);
                                        
            if (order is null)
            {
                return Errors.Orders.OrderNotFound;
            }

            var orderItem = order.Items.FirstOrDefault(x => x.Id == request.OrderItemId);

            if (orderItem is null)
            {
                return Errors.Orders.OrderItemNotFound;
            }

            orderItem.Quantity = request.Quantity;

            order.Update();

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return orderItem!.ToDto();
        }
    }
}
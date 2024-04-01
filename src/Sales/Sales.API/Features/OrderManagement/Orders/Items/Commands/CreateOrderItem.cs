using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Domain.Infrastructure;
using YourBrand.Sales.Features.OrderManagement.Orders.Dtos;
using YourBrand.Sales.Features.OrderManagement.Repositories;

using static YourBrand.Sales.Results;
using static YourBrand.Sales.Domain.Errors.Orders;

namespace YourBrand.Sales.Features.OrderManagement.Orders.Items.Commands;

public sealed record CreateOrderItem(string OrderId, string Description, string? ProductId, Guid? SubscriptionPlanId, double Quantity, string? Unit, decimal UnitPrice, decimal? RegularPrice, double? VatRate, decimal? Discount, string? Notes) : IRequest<Result<OrderItemDto>>
{
    public sealed class Validator : AbstractValidator<CreateOrderItem>
    {
        public Validator()
        {
            RuleFor(x => x.OrderId);

            RuleFor(x => x.Description).NotEmpty().MaximumLength(240);
        }
    }

    public sealed class Handler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
        : IRequestHandler<CreateOrderItem, Result<OrderItemDto>>
    {
        public async Task<Result<OrderItemDto>> Handle(CreateOrderItem request, CancellationToken cancellationToken)
        {
            var order = await orderRepository.FindByIdAsync(request.OrderId, cancellationToken);

            if (order is null)
            {
                return OrderNotFound;
            }

            var orderItem = order.AddItem(request.Description, request.ProductId, request.UnitPrice, request.RegularPrice, null, request.Discount, request.Quantity, request.Unit, request.VatRate, request.Notes);

            orderItem.SubscriptionPlanId = request.SubscriptionPlanId;

            order.Update();

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return orderItem!.ToDto();
        }
    }
}

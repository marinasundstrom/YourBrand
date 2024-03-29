using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Domain.Infrastructure;
using YourBrand.Sales.Features.OrderManagement.Orders.Dtos;
using YourBrand.Sales.Features.OrderManagement.Repositories;
using YourBrand.Sales.Persistence;

namespace YourBrand.Sales.Features.OrderManagement.Orders.Items.Commands;

public sealed record UpdateOrderItem(string OrderId, string OrderItemId, string Description, string? ProductId, Guid? SubscriptionPlanId, double Quantity, string? Unit, decimal UnitPrice, decimal? RegularPrice, double VatRate, decimal? Discount, string? Notes) : IRequest<Result<OrderItemDto>>
{
    public sealed class Validator : AbstractValidator<UpdateOrderItem>
    {
        public Validator()
        {
            RuleFor(x => x.OrderId);

            RuleFor(x => x.Description).NotEmpty().MaximumLength(240);
        }
    }

    public sealed class Handler(IOrderRepository orderRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateOrderItem, Result<OrderItemDto>>
    {
        private readonly IOrderRepository orderRepository = orderRepository;
        private readonly IUnitOfWork unitOfWork = unitOfWork;

        public async Task<Result<OrderItemDto>> Handle(UpdateOrderItem request, CancellationToken cancellationToken)
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

            orderItem.Description = request.Description;
            orderItem.ProductId = request.ProductId;
            orderItem.SubscriptionPlanId = request.SubscriptionPlanId;
            orderItem.Unit = request.Unit;
            orderItem.Price = request.UnitPrice;
            orderItem.RegularPrice = request.RegularPrice;
            orderItem.Discount = request.RegularPrice - request.UnitPrice;
            orderItem.VatRate = request.VatRate;
            orderItem.Quantity = request.Quantity;
            orderItem.Notes = request.Notes;

            orderItem.Update();
            order.Update();

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return orderItem!.ToDto();
        }
    }
}

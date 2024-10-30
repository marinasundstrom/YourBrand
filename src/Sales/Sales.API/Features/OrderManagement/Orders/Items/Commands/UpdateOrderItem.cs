﻿using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Features.OrderManagement.Orders.Dtos;
using YourBrand.Sales.Features.OrderManagement.Repositories;

namespace YourBrand.Sales.Features.OrderManagement.Orders.Items.Commands;

public sealed record UpdateOrderItem(string OrganizationId, string OrderId, string OrderItemId, string Description, string? ProductId, Guid? SubscriptionPlanId, double Quantity, string? Unit, decimal UnitPrice, decimal? RegularPrice, double VatRate, decimal? Discount, string? Notes) : IRequest<Result<OrderItemDto>>
{
    public sealed class Validator : AbstractValidator<UpdateOrderItem>
    {
        public Validator()
        {
            RuleFor(x => x.OrderId);

            RuleFor(x => x.Description).NotEmpty().MaximumLength(240);
        }
    }

    public sealed class Handler(TimeProvider timeProvider, IOrderRepository orderRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateOrderItem, Result<OrderItemDto>>
    {
        public async Task<Result<OrderItemDto>> Handle(UpdateOrderItem request, CancellationToken cancellationToken)
        {
            var order = await orderRepository
                                        .GetAll()
                                        .InOrganization(request.OrganizationId)
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

            orderItem.Description = request.Description;
            orderItem.ProductId = request.ProductId;
            orderItem.SubscriptionPlanId = request.SubscriptionPlanId;
            orderItem.Unit = request.Unit;
            orderItem.Price = request.UnitPrice;
            orderItem.RegularPrice = request.RegularPrice;

            // Foo
            //orderItem.Discount = request.RegularPrice - request.UnitPrice;

            orderItem.VatRate = request.VatRate;
            orderItem.Quantity = request.Quantity;
            orderItem.Notes = request.Notes;

            orderItem.Update(timeProvider);
            order.Update(timeProvider);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return orderItem!.ToDto();
        }
    }
}
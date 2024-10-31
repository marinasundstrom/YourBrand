﻿using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Features.OrderManagement.Orders.Dtos;
using YourBrand.Sales.Features.OrderManagement.Repositories;

using static YourBrand.Sales.Domain.Errors.Orders;

namespace YourBrand.Sales.Features.OrderManagement.Orders.Items.Commands;

public sealed record CreateOrderItem(string OrganizationId, string OrderId, string Description, string? ProductId, Guid? SubscriptionPlanId, double Quantity, string? Unit, decimal UnitPrice, decimal? RegularPrice, double? VatRate, decimal? Discount, string? Notes) : IRequest<Result<OrderItemDto>>
{
    public sealed class Validator : AbstractValidator<CreateOrderItem>
    {
        public Validator()
        {
            RuleFor(x => x.OrderId);

            RuleFor(x => x.Description).NotEmpty().MaximumLength(240);
        }
    }

    public sealed class Handler(TimeProvider timeProvider, IOrderRepository orderRepository, IUnitOfWork unitOfWork)
        : IRequestHandler<CreateOrderItem, Result<OrderItemDto>>
    {
        public async Task<Result<OrderItemDto>> Handle(CreateOrderItem request, CancellationToken cancellationToken)
        {
            var order = await orderRepository
                                        .GetAll()
                                        .InOrganization(request.OrganizationId)
                                        .FirstOrDefaultAsync(x => x.Id == request.OrderId, cancellationToken);

            if (order is null)
            {
                return OrderNotFound;
            }

            var orderItem = order.AddItem(
                request.Description,
                request.ProductId,
                request.UnitPrice,
                request.RegularPrice,
                null,
                request.Discount,
                request.Quantity,
                request.Unit,
                request.VatRate,
                request.Notes,
                timeProvider);

            orderItem.SubscriptionPlanId = request.SubscriptionPlanId;

            order.Update(timeProvider);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return orderItem!.ToDto();
        }
    }
}
using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Features.OrderManagement.Orders.Dtos;
using YourBrand.Sales.Features.OrderManagement.Repositories;

using static YourBrand.Sales.Domain.Errors.Orders;

namespace YourBrand.Sales.Features.OrderManagement.Orders.Items.Options;

public sealed record CreateOrderItemOption(string OrganizationId, string OrderId, string OrderItemId, string Description, string? ProductId, string? ItemId, decimal? Price, decimal? Discount) : IRequest<Result<OrderItemOptionDto>>
{
    public sealed class Validator : AbstractValidator<CreateOrderItemOption>
    {
        public Validator()
        {
            RuleFor(x => x.OrderId);

            RuleFor(x => x.Description).NotEmpty().MaximumLength(240);
        }
    }

    public sealed class Handler(TimeProvider timeProvider, IOrderRepository orderRepository, IUnitOfWork unitOfWork)
        : IRequestHandler<CreateOrderItemOption, Result<OrderItemOptionDto>>
    {
        public async Task<Result<OrderItemOptionDto>> Handle(CreateOrderItemOption request, CancellationToken cancellationToken)
        {
            var order = await orderRepository
                                        .GetAll()
                                        .InOrganization(request.OrganizationId)
                                        .FirstOrDefaultAsync(x => x.Id == request.OrderId, cancellationToken);

            if (order is null)
            {
                return OrderNotFound;
            }

            var orderItem = order.Items.FirstOrDefault(x => x.Id == request.OrderItemId);

            if (orderItem is null)
            {
                return Errors.Orders.OrderItemNotFound;
            }

            var option = orderItem.AddOption(request.Description, request.ProductId, request.ItemId, request.Price, request.Discount, timeProvider);

            order.Update(timeProvider);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return option!.ToDto();
        }
    }
}
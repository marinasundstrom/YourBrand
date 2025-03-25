using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Features.OrderManagement.Orders.Dtos;
using YourBrand.Sales.Features.OrderManagement.Repositories;

namespace YourBrand.Sales.Features.OrderManagement.Orders.Items.Options;

public sealed record UpdateOrderItemOption(string OrganizationId, string OrderId, string OrderItemId, string OptionId, string Description, string? ProductId, string? ItemId, decimal? Price, decimal? Discount) : IRequest<Result<OrderItemOptionDto>>
{
    public sealed class Validator : AbstractValidator<UpdateOrderItemOption>
    {
        public Validator()
        {
            RuleFor(x => x.OrderId);

            RuleFor(x => x.Description).NotEmpty().MaximumLength(240);
        }
    }

    public sealed class Handler(TimeProvider timeProvider, IOrderRepository orderRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateOrderItemOption, Result<OrderItemOptionDto>>
    {
        public async Task<Result<OrderItemOptionDto>> Handle(UpdateOrderItemOption request, CancellationToken cancellationToken)
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

            var option = orderItem.Options.FirstOrDefault(x => x.Id == request.OptionId);

            if (option is null)
            {
                return Errors.Orders.OrderItemNotFound;
            }

            /*

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
            */

            orderItem.Update(timeProvider);
            order.Update(timeProvider);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return option!.ToDto();
        }
    }
}
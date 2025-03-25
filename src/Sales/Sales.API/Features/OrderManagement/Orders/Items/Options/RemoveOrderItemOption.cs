using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Features.OrderManagement.Repositories;

using static YourBrand.Sales.Domain.Errors.Orders;
using static YourBrand.Result;

namespace YourBrand.Sales.Features.OrderManagement.Orders.Items.Options;

public sealed record RemoveOrderItemOption(string OrganizationId, string OrderId, string OrderItemId, string OptionId) : IRequest<Result>
{
    public sealed class Validator : AbstractValidator<RemoveOrderItemOption>
    {
        public Validator()
        {
            RuleFor(x => x.OrderId).NotEmpty();

            RuleFor(x => x.OrderItemId).NotEmpty();
        }
    }

    public sealed class Handler(TimeProvider timeProvider, IOrderRepository orderRepository, IUnitOfWork unitOfWork) : IRequestHandler<RemoveOrderItemOption, Result>
    {
        public async Task<Result> Handle(RemoveOrderItemOption request, CancellationToken cancellationToken)
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
                return OrderItemNotFound;
            }

            var option = orderItem.Options.FirstOrDefault(x => x.Id == request.OptionId);

            if (option is null)
            {
                return OrderItemNotFound;
            }

            orderItem.RemoveOption(option);

            order.Update(timeProvider);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Success;
        }
    }
}
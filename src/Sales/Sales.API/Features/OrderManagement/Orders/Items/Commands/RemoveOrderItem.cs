using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Features.OrderManagement.Repositories;

using static YourBrand.Sales.Domain.Errors.Orders;
using static YourBrand.Sales.Results;

namespace YourBrand.Sales.Features.OrderManagement.Orders.Items.Commands;

public sealed record RemoveOrderItem(string OrganizationId, string OrderId, string OrderItemId) : IRequest<Result>
{
    public sealed class Validator : AbstractValidator<RemoveOrderItem>
    {
        public Validator()
        {
            RuleFor(x => x.OrderId).NotEmpty();

            RuleFor(x => x.OrderItemId).NotEmpty();
        }
    }

    public sealed class Handler(IOrderRepository orderRepository, IUnitOfWork unitOfWork) : IRequestHandler<RemoveOrderItem, Result>
    {
        public async Task<Result> Handle(RemoveOrderItem request, CancellationToken cancellationToken)
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

            order.RemoveOrderItem(orderItem);

            order.Update();

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Success;
        }
    }
}
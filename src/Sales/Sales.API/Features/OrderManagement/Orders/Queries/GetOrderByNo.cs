using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Features.OrderManagement.Orders.Dtos;
using YourBrand.Sales.Features.OrderManagement.Repositories;

namespace YourBrand.Sales.Features.OrderManagement.Orders.Queries;

public record GetOrderByNo(string OrganizationId, int OrderNo) : IRequest<Result<OrderDto>>
{
    public class Validator : AbstractValidator<GetOrderById>
    {
        public Validator()
        {
            RuleFor(x => x.Id);
        }
    }

    public class Handler(IOrderRepository orderRepository) : IRequestHandler<GetOrderByNo, Result<OrderDto>>
    {
        public async Task<Result<OrderDto>> Handle(GetOrderByNo request, CancellationToken cancellationToken)
        {
            var order = await orderRepository
                            .GetAll()
                            .Where(x => x.OrganizationId == request.OrganizationId)
                            .FirstOrDefaultAsync(x => x.OrderNo == request.OrderNo, cancellationToken);

            if (order is null)
            {
                return Errors.Orders.OrderNotFound;
            }

            return order.ToDto();
        }
    }
}
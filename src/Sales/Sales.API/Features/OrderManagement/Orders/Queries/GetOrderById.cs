using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Features.OrderManagement.Orders.Dtos;
using YourBrand.Sales.Features.OrderManagement.Repositories;

namespace YourBrand.Sales.Features.OrderManagement.Orders.Queries;

public record GetOrderById(string OrganizationId, string Id) : IRequest<Result<OrderDto>>
{
    public class Validator : AbstractValidator<GetOrderById>
    {
        public Validator()
        {
            RuleFor(x => x.Id);
        }
    }

    public class Handler(IOrderRepository orderRepository) : IRequestHandler<GetOrderById, Result<OrderDto>>
    {
        public async Task<Result<OrderDto>> Handle(GetOrderById request, CancellationToken cancellationToken)
        {
            var order = await orderRepository
                            .GetAll()
                            .InOrganization(request.OrganizationId)
                            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
                            
            if (order is null)
            {
                return Errors.Orders.OrderNotFound;
            }

            return order.ToDto();
        }
    }
}
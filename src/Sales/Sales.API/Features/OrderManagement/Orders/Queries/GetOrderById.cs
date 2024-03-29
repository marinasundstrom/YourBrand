using FluentValidation;

using MediatR;

using YourBrand.Sales.Features.OrderManagement.Orders.Dtos;
using YourBrand.Sales.Features.OrderManagement.Repositories;

namespace YourBrand.Sales.Features.OrderManagement.Orders.Queries;

public record GetOrderById(string Id) : IRequest<Result<OrderDto>>
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
        private readonly IOrderRepository orderRepository = orderRepository;

        public async Task<Result<OrderDto>> Handle(GetOrderById request, CancellationToken cancellationToken)
        {
            var order = await orderRepository.FindByIdAsync(request.Id, cancellationToken);

            if (order is null)
            {
                return Errors.Orders.OrderNotFound;
            }

            return order.ToDto();
        }
    }
}
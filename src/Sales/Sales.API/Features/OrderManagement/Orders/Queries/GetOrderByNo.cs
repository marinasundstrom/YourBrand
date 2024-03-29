using FluentValidation;

using MediatR;

using YourBrand.Sales.Features.OrderManagement.Orders.Dtos;
using YourBrand.Sales.Features.OrderManagement.Repositories;

namespace YourBrand.Sales.Features.OrderManagement.Orders.Queries;

public record GetOrderByNo(int OrderNo) : IRequest<Result<OrderDto>>
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
        private readonly IOrderRepository orderRepository = orderRepository;

        public async Task<Result<OrderDto>> Handle(GetOrderByNo request, CancellationToken cancellationToken)
        {
            var order = await orderRepository.FindByNoAsync(request.OrderNo, cancellationToken);

            if (order is null)
            {
                return Errors.Orders.OrderNotFound;
            }

            return order.ToDto();
        }
    }
}
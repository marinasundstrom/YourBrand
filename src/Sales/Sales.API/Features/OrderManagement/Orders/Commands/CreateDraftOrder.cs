using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Domain.Infrastructure;
using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Domain.Events;
using YourBrand.Sales.Features.OrderManagement.Orders.Dtos;
using YourBrand.Sales.Features.OrderManagement.Repositories;
using YourBrand.Sales.Persistence;
using YourBrand.Sales.Persistence.Repositories.Mocks;

namespace YourBrand.Sales.Features.OrderManagement.Orders.Commands;

public sealed record CreateDraftOrder(string OrganizationId) : IRequest<Result<OrderDto>>
{
    public sealed class Validator : AbstractValidator<CreateDraftOrder>
    {
        public Validator()
        {
            //RuleFor(x => x.Title).NotEmpty().MaximumLength(60);

            //RuleFor(x => x.Description).MaximumLength(240);
        }
    }

    public sealed class Handler(SalesContext salesContext, IOrderRepository orderRepository, TimeProvider timeProvider, OrderNumberFetcher orderNumberFetcher, IUnitOfWork unitOfWork, IDomainEventDispatcher domainEventDispatcher) : IRequestHandler<CreateDraftOrder, Result<OrderDto>>
    {
        public async Task<Result<OrderDto>> Handle(CreateDraftOrder request, CancellationToken cancellationToken)
        {
            var order = new Order(
                request.OrganizationId,
                typeId: 1,
                true
            );

            //order.UpdateStatus(1, timeProvider);

            orderRepository.Add(order);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            await domainEventDispatcher.Dispatch(new OrderCreated(order.Id), cancellationToken);

            Console.WriteLine("Foo: " + order.Id);

            order = await salesContext.Orders
                .IncludeAll()
                .FirstAsync(x => x.Id == order.Id, cancellationToken);

            return order!.ToDto();
        }
    }
}
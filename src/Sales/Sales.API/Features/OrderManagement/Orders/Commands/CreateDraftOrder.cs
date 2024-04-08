using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Domain.Infrastructure;
using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Domain.Events;
using YourBrand.Sales.Features.OrderManagement.Orders.Dtos;
using YourBrand.Sales.Features.OrderManagement.Repositories;

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

    public sealed class Handler(IOrderRepository orderRepository, IUnitOfWork unitOfWork, IDomainEventDispatcher domainEventDispatcher) : IRequestHandler<CreateDraftOrder, Result<OrderDto>>
    {
        public async Task<Result<OrderDto>> Handle(CreateDraftOrder request, CancellationToken cancellationToken)
        {
            var order = new Order();

            try
            {
                order.OrderNo = (await orderRepository
                    .GetAll()
                    .InOrganization(request.OrganizationId)
                    .MaxAsync(x => x.OrderNo)) + 1;
            }
            catch (InvalidOperationException e)
            {
                order.OrderNo = 1; // Order start number
            }

            order.OrganizationId = request.OrganizationId;

            order.VatIncluded = true;

            orderRepository.Add(order);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            await domainEventDispatcher.Dispatch(new OrderCreated(order.Id), cancellationToken);

            order = await orderRepository.GetAll()
                .Include(i => i.CreatedBy)
                .Include(i => i.LastModifiedBy)
                .FirstAsync(x => x.OrderNo == order.OrderNo, cancellationToken);

            return order!.ToDto();
        }
    }
}
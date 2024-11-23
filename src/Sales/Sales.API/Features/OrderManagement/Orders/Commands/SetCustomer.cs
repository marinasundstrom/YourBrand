using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Features.OrderManagement.Repositories;

namespace YourBrand.Sales.Features.OrderManagement.Orders.Commands;

public sealed record SetCustomer(string OrganizationId, string Id, string CustomerId, string Name) : IRequest<Result>
{
    public sealed class Validator : AbstractValidator<SetCustomer>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public sealed class Handler(IOrderRepository orderRepository, IUnitOfWork unitOfWork) : IRequestHandler<SetCustomer, Result>
    {
        public async Task<Result> Handle(SetCustomer request, CancellationToken cancellationToken)
        {
            var order = await orderRepository
                .GetAll()
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (order is null)
            {
                return Errors.Orders.OrderNotFound;
            }

            if (order.Customer is null)
            {
                order.Customer = new Domain.Entities.Customer();
            }

            order.Customer.Id = request.CustomerId;
            order.Customer.Name = request.Name;

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}
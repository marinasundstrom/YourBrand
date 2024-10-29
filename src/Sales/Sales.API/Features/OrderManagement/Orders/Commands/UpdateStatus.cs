using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Features.OrderManagement.Repositories;

namespace YourBrand.Sales.Features.OrderManagement.Orders.Commands;

public sealed record UpdateStatus(string OrganizationId, string Id, int StatusId) : IRequest<Result>
{
    public sealed class Validator : AbstractValidator<UpdateStatus>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public sealed class Handler(IOrderRepository orderRepository, IUnitOfWork unitOfWork, TimeProvider timeProvider) : IRequestHandler<UpdateStatus, Result>
    {
        public async Task<Result> Handle(UpdateStatus request, CancellationToken cancellationToken)
        {
            var order = await orderRepository
                            .GetAll()
                            .InOrganization(request.OrganizationId)
                            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (order is null)
            {
                return Errors.Orders.OrderNotFound;
            }

            order.UpdateStatus(request.StatusId, timeProvider);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Results.Success;
        }
    }
}
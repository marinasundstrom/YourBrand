using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Features.OrderManagement.Repositories;

namespace YourBrand.Sales.Features.OrderManagement.Orders.Commands;

public sealed record UpdateAssignedUser(string OrganizationId, string Id, string? UserId) : IRequest<Result>
{
    public sealed class Validator : AbstractValidator<UpdateAssignedUser>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public sealed class Handler(IOrderRepository orderRepository, IUserRepository userRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateAssignedUser, Result>
    {
        public async Task<Result> Handle(UpdateAssignedUser request, CancellationToken cancellationToken)
        {
            var order = await orderRepository
                            .GetAll()
                            .InOrganization(request.OrganizationId)
                            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (order is null)
            {
                return Errors.Orders.OrderNotFound;
            }

            if (request.UserId is not null)
            {
                var user = await userRepository.FindByIdAsync(request.UserId, cancellationToken);

                if (user is null)
                {
                    return Errors.Users.UserNotFound;
                }
            }

            order.UpdateAssigneeId(request.UserId);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Results.Success;
        }
    }
}
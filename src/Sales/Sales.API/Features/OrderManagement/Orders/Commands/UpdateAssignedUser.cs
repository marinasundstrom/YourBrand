using FluentValidation;

using MediatR;

using YourBrand.Sales.Features.OrderManagement.Orders.Dtos;
using YourBrand.Sales.Features.OrderManagement.Repositories;

namespace YourBrand.Sales.Features.OrderManagement.Orders.Commands;

public sealed record UpdateAssignedUser(string Id, string? UserId) : IRequest<Result>
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
        private readonly IOrderRepository orderRepository = orderRepository;
        private readonly IUserRepository userRepository = userRepository;
        private readonly IUnitOfWork unitOfWork = unitOfWork;

        public async Task<Result> Handle(UpdateAssignedUser request, CancellationToken cancellationToken)
        {
            var order = await orderRepository.FindByIdAsync(request.Id, cancellationToken);

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
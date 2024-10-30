using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Domain.ValueObjects;
using YourBrand.Sales.Features.OrderManagement.Orders.Dtos;
using YourBrand.Sales.Features.OrderManagement.Repositories;

namespace YourBrand.Sales.Features.OrderManagement.Orders.Commands;

public sealed record UpdateShippingDetails(string OrganizationId, string Id, ShippingDetailsDto ShippingDetails) : IRequest<Result>
{
    public sealed class Validator : AbstractValidator<UpdateShippingDetails>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public sealed class Handler(IOrderRepository orderRepository, IUserRepository userRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateShippingDetails, Result>
    {
        public async Task<Result> Handle(UpdateShippingDetails request, CancellationToken cancellationToken)
        {
            var order = await orderRepository
                            .GetAll()
                            .InOrganization(request.OrganizationId)
                            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (order is null)
            {
                return Errors.Orders.OrderNotFound;
            }

            var shippingDetails = order.ShippingDetails ??= new ShippingDetails();

            shippingDetails.FirstName = request.ShippingDetails.FirstName;
            shippingDetails.LastName = request.ShippingDetails.LastName;
            //SSN = request.ShippingDetails.SSN,
            //Email = request.ShippingDetails.Email,
            //PhoneNumber = request.ShippingDetails.PhoneNumber,
            shippingDetails.Address = request.ShippingDetails.Address.MapOntoAddress(shippingDetails.Address ??= new Address());

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Results.Success;
        }
    }
}
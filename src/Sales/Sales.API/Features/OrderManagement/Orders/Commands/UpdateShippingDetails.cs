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
            shippingDetails.Address = Map(shippingDetails.Address ??= new Address(), request.ShippingDetails.Address);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Results.Success;
        }

        private Domain.ValueObjects.Address Map(Domain.ValueObjects.Address a, AddressDto address)
        {
            a.Thoroughfare = address.Thoroughfare;
            a.Premises = address.Premises;
            a.SubPremises = address.SubPremises;
            a.PostalCode = address.PostalCode;
            a.Locality = address.Locality;
            a.SubAdministrativeArea = address.SubAdministrativeArea;
            a.AdministrativeArea = address.AdministrativeArea;
            a.Country = address.Country;

            return a;
        }
    }
}
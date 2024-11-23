using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Invoicing.Domain;
using YourBrand.Invoicing.Domain.Entities;

namespace YourBrand.Invoicing.Application.Commands;

public sealed record UpdateShippingDetails(string OrganizationId, string Id, ShippingDetailsDto ShippingDetails) : IRequest<Result>
{
    public sealed class Handler(IInvoicingContext context) : IRequestHandler<UpdateShippingDetails, Result>
    {
        private readonly IInvoicingContext _context = context;

        public async Task<Result> Handle(UpdateShippingDetails request, CancellationToken cancellationToken)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Items)
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (invoice is null)
            {
                return Errors.Invoices.InvoiceNotFound;
            }

            var shippingDetails = invoice.ShippingDetails ??= new ShippingDetails();

            shippingDetails.FirstName = request.ShippingDetails.FirstName;
            shippingDetails.LastName = request.ShippingDetails.LastName;
            //SSN = request.ShippingDetails.SSN,
            //Email = request.ShippingDetails.Email,
            //PhoneNumber = request.ShippingDetails.PhoneNumber,
            shippingDetails.Address = Map(shippingDetails.Address ??= new Address(), request.ShippingDetails.Address);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }

        private Address Map(Address a, AddressDto address)
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
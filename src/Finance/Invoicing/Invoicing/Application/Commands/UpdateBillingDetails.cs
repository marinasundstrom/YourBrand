using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Invoicing.Domain;
using YourBrand.Invoicing.Domain.Entities;

namespace YourBrand.Invoicing.Application.Commands;


public sealed record UpdateBillingDetails(string OrganizationId, string Id, BillingDetailsDto BillingDetails) : IRequest<Result>
{
    public sealed class Handler(IInvoicingContext context) : IRequestHandler<UpdateBillingDetails, Result>
    {
        private readonly IInvoicingContext _context = context;

        public async Task<Result> Handle(UpdateBillingDetails request, CancellationToken cancellationToken)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Items)
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (invoice is null)
            {
                return Errors.Invoices.InvoiceNotFound;
            }

            var billingDetails = invoice.BillingDetails ??= new BillingDetails();

            billingDetails.FirstName = request.BillingDetails.FirstName;
            billingDetails.LastName = request.BillingDetails.LastName;
            billingDetails.SSN = request.BillingDetails.SSN;
            billingDetails.Email = request.BillingDetails.Email;
            billingDetails.PhoneNumber = request.BillingDetails.PhoneNumber;
            billingDetails.Address = Map(billingDetails.Address ??= new Address(), request.BillingDetails.Address);

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
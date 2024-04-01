using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Invoicing.Domain;

namespace YourBrand.Invoicing.Application.Commands;

public sealed record SetCustomer(string Id, string CustomerId, string Name) : IRequest<Result>
{
    public sealed class Validator : AbstractValidator<SetCustomer>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public sealed class Handler(IInvoicingContext context) : IRequestHandler<SetCustomer, Result>
    {
        public async Task<Result> Handle(SetCustomer request, CancellationToken cancellationToken)
        {
            var invoice = await context.Invoices
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (invoice is null)
            {
                throw new Exception();
            }

            if (invoice.Customer is null)
            {
                invoice.Customer = new Domain.Entities.Customer();
            }

            invoice.Customer.Id = request.CustomerId;
            invoice.Customer.Name = request.Name;

            await context.SaveChangesAsync(cancellationToken);

            return Results.Success;
        }
    }
}
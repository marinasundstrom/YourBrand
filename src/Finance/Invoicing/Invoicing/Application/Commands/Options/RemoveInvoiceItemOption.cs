using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Invoicing.Domain;

using static YourBrand.Invoicing.Domain.Errors.Invoices;
using static YourBrand.Result;

namespace YourBrand.Invoicing.Features.InvoiceManagement.Invoices.Items.Options;

public sealed record RemoveInvoiceItemOption(string OrganizationId, string InvoiceId, string InvoiceItemId, string OptionId) : IRequest<Result>
{
    public sealed class Validator : AbstractValidator<RemoveInvoiceItemOption>
    {
        public Validator()
        {
            RuleFor(x => x.InvoiceId).NotEmpty();

            RuleFor(x => x.InvoiceItemId).NotEmpty();
        }
    }

    public sealed class Handler(TimeProvider timeProvider, IInvoicingContext invoicingContext) : IRequestHandler<RemoveInvoiceItemOption, Result>
    {
        public async Task<Result> Handle(RemoveInvoiceItemOption request, CancellationToken cancellationToken)
        {
            var order = await invoicingContext.Invoices
                                        .InOrganization(request.OrganizationId)
                                        .FirstOrDefaultAsync(x => x.Id == request.InvoiceId, cancellationToken);

            if (order is null)
            {
                return InvoiceNotFound;
            }

            var orderItem = order.Items.FirstOrDefault(x => x.Id == request.InvoiceItemId);

            if (orderItem is null)
            {
                return InvoiceItemNotFound;
            }

            var option = orderItem.Options.FirstOrDefault(x => x.Id == request.OptionId);

            if (option is null)
            {
                return InvoiceItemNotFound;
            }

            orderItem.RemoveOption(option);

            order.Update(); //timeProvider);

            await invoicingContext.SaveChangesAsync(cancellationToken);

            return Success;
        }
    }
}
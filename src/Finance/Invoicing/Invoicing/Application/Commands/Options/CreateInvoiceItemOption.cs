using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Invoicing.Application;
using YourBrand.Invoicing.Domain;

using static YourBrand.Invoicing.Domain.Errors.Invoices;

namespace YourBrand.Invoicing.Features.InvoiceManagement.Invoices.Items.Options;

public sealed record CreateInvoiceItemOption(string OrganizationId, string InvoiceId, string InvoiceItemId, string Description, string? ProductId, string? ItemId, decimal? Price, decimal? Discount) : IRequest<Result<InvoiceItemOptionDto>>
{
    public sealed class Validator : AbstractValidator<CreateInvoiceItemOption>
    {
        public Validator()
        {
            RuleFor(x => x.InvoiceId);

            RuleFor(x => x.Description).NotEmpty().MaximumLength(240);
        }
    }

    public sealed class Handler(TimeProvider timeProvider, IInvoicingContext invoicingContext)
        : IRequestHandler<CreateInvoiceItemOption, Result<InvoiceItemOptionDto>>
    {
        public async Task<Result<InvoiceItemOptionDto>> Handle(CreateInvoiceItemOption request, CancellationToken cancellationToken)
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
                return Errors.Invoices.InvoiceItemNotFound;
            }

            var option = orderItem.AddOption(request.Description, request.ProductId, request.ItemId, request.Price, request.Discount);

            order.Update(); //timeProvider);

            await invoicingContext.SaveChangesAsync(cancellationToken);

            return option!.ToDto();
        }
    }
}
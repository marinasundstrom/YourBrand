using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Invoicing.Application;
using YourBrand.Invoicing.Domain;

namespace YourBrand.Invoicing.Features.InvoiceManagement.Invoices.Items.Options;

public sealed record UpdateInvoiceItemOption(string OrganizationId, string InvoiceId, string InvoiceItemId, string OptionId, string Description, string? ProductId, string? ItemId, decimal? Price, decimal? Discount) : IRequest<Result<InvoiceItemOptionDto>>
{
    public sealed class Validator : AbstractValidator<UpdateInvoiceItemOption>
    {
        public Validator()
        {
            RuleFor(x => x.InvoiceId);

            RuleFor(x => x.Description).NotEmpty().MaximumLength(240);
        }
    }

    public sealed class Handler(TimeProvider timeProvider, IInvoicingContext invoicingContext) : IRequestHandler<UpdateInvoiceItemOption, Result<InvoiceItemOptionDto>>
    {
        public async Task<Result<InvoiceItemOptionDto>> Handle(UpdateInvoiceItemOption request, CancellationToken cancellationToken)
        {
            var order = await invoicingContext.Invoices
                                        .InOrganization(request.OrganizationId)
                                        .FirstOrDefaultAsync(x => x.Id == request.InvoiceId, cancellationToken);

            if (order is null)
            {
                return Errors.Invoices.InvoiceNotFound;
            }

            var orderItem = order.Items.FirstOrDefault(x => x.Id == request.InvoiceItemId);

            if (orderItem is null)
            {
                return Errors.Invoices.InvoiceItemNotFound;
            }

            var option = orderItem.Options.FirstOrDefault(x => x.Id == request.OptionId);

            if (option is null)
            {
                return Errors.Invoices.InvoiceItemNotFound;
            }

            /*

            orderItem.Description = request.Description;
            orderItem.ProductId = request.ProductId;
            orderItem.SubscriptionPlanId = request.SubscriptionPlanId;
            orderItem.Unit = request.Unit;
            orderItem.Price = request.UnitPrice;
            orderItem.RegularPrice = request.RegularPrice;

            // Foo
            //orderItem.Discount = request.RegularPrice - request.UnitPrice;

            orderItem.VatRate = request.VatRate;
            orderItem.Quantity = request.Quantity;
            orderItem.Notes = request.Notes;
            */

            orderItem.Update(); //timeProvider);
            order.Update(); //timeProvider);

            await invoicingContext.SaveChangesAsync(cancellationToken);

            return option!.ToDto();
        }
    }
}
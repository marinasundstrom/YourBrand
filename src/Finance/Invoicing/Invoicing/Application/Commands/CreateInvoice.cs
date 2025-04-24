
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Invoicing.Domain;
using YourBrand.Invoicing.Domain.Entities;

namespace YourBrand.Invoicing.Application.Commands;

public record CreateInvoice(string OrganizationId, DateTime? Date, int? Status, string? Note, int? OrderNo, SetCustomerDto? Customer, IEnumerable<CreateInvoiceItemDto> Items) : IRequest<InvoiceDto>
{
    public class Handler(IInvoicingContext context, TimeProvider timeProvider, InvoiceNumberFetcher invoiceNumberFetcher) : IRequestHandler<CreateInvoice, InvoiceDto>
    {
        public async Task<InvoiceDto> Handle(CreateInvoice request, CancellationToken cancellationToken)
        {
            var invoice = new YourBrand.Invoicing.Domain.Entities.Invoice(request.Date, note: request.Note,
                type: Domain.Enums.InvoiceType.Invoice);

            invoice.SetCurrency("SEK");

            //invoice.Id = Guid.NewGuid().ToString();

            await invoice.AssignInvoiceNo(invoiceNumberFetcher);

            if (request.Status is not null)
            {
                invoice.UpdateStatus(request.Status.GetValueOrDefault(), timeProvider);
            }

            if (request.Customer is not null)
            {
                if (invoice.Customer is null)
                {
                    invoice.Customer = new Customer();
                }

                invoice.Customer.Id = request.Customer.Id;
                invoice.Customer.Name = request.Customer.Name;
            }

            invoice.OrganizationId = request.OrganizationId;

            if (request.OrderNo is not null)
            {
                invoice.OrderNo = request.OrderNo.GetValueOrDefault();
             }

            foreach (var orderItem in request.Items)
            {
                var item = invoice.AddItem(
                    orderItem.ProductType,
                    orderItem.Description,
                    orderItem.ProductId,
                    orderItem.UnitPrice,
                    orderItem.Unit,
                    orderItem.VatRate,
                    orderItem.Quantity,
                    timeProvider);

                if (orderItem.Options is not null)
                {
                    foreach (var option in orderItem.Options)
                    {
                        item.AddOption(option.Name, option.Description, option.Value, option.ProductId, option.ItemId, option.Price, null, timeProvider);
                    }
                }

                if (orderItem.Discounts is not null)
                {
                    foreach (var discount in orderItem.Discounts)
                    {
                        item.AddPromotionalDiscount(discount.Description, discount.Amount, discount.Rate, timeProvider);
                    }
                }
            }

            context.Invoices.Add(invoice);

            await context.SaveChangesAsync(cancellationToken);

            invoice = await context.Invoices
                .Include(i => i.Status)
                .Include(i => i.Items)
                .AsSplitQuery()
                .AsNoTracking()
                .InOrganization(request.OrganizationId)
                .FirstAsync(x => x.Id == invoice.Id, cancellationToken);

            return invoice.ToDto();
        }
    }
}

public record SetCustomerDto(string Id, string Name);
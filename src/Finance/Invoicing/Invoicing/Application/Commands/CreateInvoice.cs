
using MediatR;

using YourBrand.Invoicing.Domain;
using YourBrand.Invoicing.Domain.Enums;

namespace YourBrand.Invoicing.Application.Commands;

public record CreateInvoice(DateTime? Date, InvoiceStatus? Status, string? Note, SetCustomerDto? Customer) : IRequest<InvoiceDto>
{
    public class Handler(IInvoicingContext context) : IRequestHandler<CreateInvoice, InvoiceDto>
    {
        public async Task<InvoiceDto> Handle(CreateInvoice request, CancellationToken cancellationToken)
        {
            var invoice = new YourBrand.Invoicing.Domain.Entities.Invoice(request.Date, note: request.Note);

            invoice.Id = Guid.NewGuid().ToString();

            try
            {
                invoice.InvoiceNo = (context.Invoices.Select(x => x.InvoiceNo).ToList().Select(x => int.Parse(x)).Max() + 1).ToString();
            }
            catch
            {
                invoice.InvoiceNo = "1";
            }

            if (request.Customer is not null)
            {
                if (invoice.Customer is null)
                {
                    invoice.Customer = new Domain.Entities.Customer();
                }

                invoice.Customer.Id = request.Customer.Id;
                invoice.Customer.Name = request.Customer.Name;
            }

            context.Invoices.Add(invoice);

            await context.SaveChangesAsync(cancellationToken);

            return invoice.ToDto();
        }
    }
}

public record SetCustomerDto(string Id, string Name);
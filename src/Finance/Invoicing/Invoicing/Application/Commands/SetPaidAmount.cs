using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Invoicing.Domain;
using YourBrand.Invoicing.Domain.Enums;

namespace YourBrand.Invoicing.Application.Commands;

public record SetPaidAmount(string InvoiceId, decimal Amount) : IRequest
{
    public class Handler(IInvoicingContext context) : IRequestHandler<SetPaidAmount>
    {
        public async Task Handle(SetPaidAmount request, CancellationToken cancellationToken)
        {
            var invoice = await context.Invoices.FirstAsync(x => x.Id == request.InvoiceId, cancellationToken);

            invoice.SetPaid(request.Amount);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}

public record SetDate(string InvoiceId, DateTime Date) : IRequest
{
    public class Handler(IInvoicingContext context) : IRequestHandler<SetDate>
    {
        public async Task Handle(SetDate request, CancellationToken cancellationToken)
        {
            var invoice = await context.Invoices.FirstAsync(x => x.Id == request.InvoiceId, cancellationToken);

            invoice.SetIssueDate(request.Date);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}

public record SetType(string InvoiceId, InvoiceType Type) : IRequest
{
    public class Handler(IInvoicingContext context) : IRequestHandler<SetType>
    {
        public async Task Handle(SetType request, CancellationToken cancellationToken)
        {
            var invoice = await context.Invoices.FirstAsync(x => x.Id == request.InvoiceId, cancellationToken);

            invoice.SetType(request.Type);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}

public record SetDueDate(string InvoiceId, DateTime DueDate) : IRequest
{
    public class Handler(IInvoicingContext context) : IRequestHandler<SetDueDate>
    {
        public async Task Handle(SetDueDate request, CancellationToken cancellationToken)
        {
            var invoice = await context.Invoices.FirstAsync(x => x.Id == request.InvoiceId, cancellationToken);

            invoice.SetDueDate(request.DueDate);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}

public record SetReference(string InvoiceId, string? Reference) : IRequest
{
    public class Handler(IInvoicingContext context) : IRequestHandler<SetReference>
    {
        public async Task Handle(SetReference request, CancellationToken cancellationToken)
        {
            var invoice = await context.Invoices.FirstAsync(x => x.Id == request.InvoiceId, cancellationToken);

            invoice.SetReference(request.Reference);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}

public record SetNote(string InvoiceId, string? Note) : IRequest
{
    public class Handler(IInvoicingContext context) : IRequestHandler<SetNote>
    {
        public async Task Handle(SetNote request, CancellationToken cancellationToken)
        {
            var invoice = await context.Invoices.FirstAsync(x => x.Id == request.InvoiceId, cancellationToken);

            invoice.SetNote(request.Note);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}
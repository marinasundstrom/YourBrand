using YourBrand.Invoicing.Domain;
using YourBrand.Invoicing.Domain.Enums;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Invoicing.Application.Commands;

public record SetPaidAmount(int InvoiceId, decimal Amount) : IRequest
{
    public class Handler : IRequestHandler<SetPaidAmount>
    {
        private readonly IInvoicingContext _context;

        public Handler(IInvoicingContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(SetPaidAmount request, CancellationToken cancellationToken)
        {
            var invoice = await _context.Invoices.FirstAsync(x => x.Id == request.InvoiceId, cancellationToken);

            invoice.SetPaid(request.Amount);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}

public record SetDate(int InvoiceId, DateTime Date) : IRequest
{
    public class Handler : IRequestHandler<SetDate>
    {
        private readonly IInvoicingContext _context;

        public Handler(IInvoicingContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(SetDate request, CancellationToken cancellationToken)
        {
            var invoice = await _context.Invoices.FirstAsync(x => x.Id == request.InvoiceId, cancellationToken);

            invoice.SetDate(request.Date);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}

public record SetType(int InvoiceId, InvoiceType Type) : IRequest
{
    public class Handler : IRequestHandler<SetType>
    {
        private readonly IInvoicingContext _context;

        public Handler(IInvoicingContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(SetType request, CancellationToken cancellationToken)
        {
            var invoice = await _context.Invoices.FirstAsync(x => x.Id == request.InvoiceId, cancellationToken);

            invoice.SetType(request.Type);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}

public record SetDueDate(int InvoiceId, DateTime DueDate) : IRequest
{
    public class Handler : IRequestHandler<SetDueDate>
    {
        private readonly IInvoicingContext _context;

        public Handler(IInvoicingContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(SetDueDate request, CancellationToken cancellationToken)
        {
            var invoice = await _context.Invoices.FirstAsync(x => x.Id == request.InvoiceId, cancellationToken);

            invoice.SetDueDate(request.DueDate);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}

public record SetReference(int InvoiceId, string? Reference) : IRequest
{
    public class Handler : IRequestHandler<SetReference>
    {
        private readonly IInvoicingContext _context;

        public Handler(IInvoicingContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(SetReference request, CancellationToken cancellationToken)
        {
            var invoice = await _context.Invoices.FirstAsync(x => x.Id == request.InvoiceId, cancellationToken);

            invoice.SetReference(request.Reference);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}

public record SetNote(int InvoiceId, string? Note) : IRequest
{
    public class Handler : IRequestHandler<SetNote>
    {
        private readonly IInvoicingContext _context;

        public Handler(IInvoicingContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(SetNote request, CancellationToken cancellationToken)
        {
            var invoice = await _context.Invoices.FirstAsync(x => x.Id == request.InvoiceId, cancellationToken);

            invoice.SetNote(request.Note);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
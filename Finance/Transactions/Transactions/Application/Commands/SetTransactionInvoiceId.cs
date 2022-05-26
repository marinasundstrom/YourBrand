using MediatR;

using Microsoft.EntityFrameworkCore;

using Transactions.Domain;

namespace Transactions.Application.Commands;

public record SetTransactionInvoiceId(string TransactionId, int InvoiceId) : IRequest
{
    public class Handler : IRequestHandler<SetTransactionInvoiceId>
    {
        private readonly ITransactionsContext _context;
        
        public Handler(ITransactionsContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(SetTransactionInvoiceId request, CancellationToken cancellationToken)
        {
            var transaction = await _context.Transactions.FirstAsync(x => x.Id == request.TransactionId, cancellationToken);

            transaction.SetInvoiceId(request.InvoiceId);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Transactions.Domain;
using YourBrand.Transactions.Domain.Enums;

namespace YourBrand.Transactions.Application.Commands;

public record SetTransactionStatus(string TransactionId, TransactionStatus Status) : IRequest
{
    public class Handler : IRequestHandler<SetTransactionStatus>
    {
        private readonly ITransactionsContext _context;

        public Handler(ITransactionsContext context)
        {
            _context = context;
        }

        public async Task Handle(SetTransactionStatus request, CancellationToken cancellationToken)
        {
            var transaction = await _context.Transactions.FirstAsync(x => x.Id == request.TransactionId, cancellationToken);

            transaction.SetStatus(request.Status);

            await _context.SaveChangesAsync(cancellationToken);

        }
    }
}
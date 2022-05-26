
using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Transactions.Domain;

namespace Transactions.Application.Commands;

public record UpdateTransactionReference(string TransactionId, string Reference) : IRequest
{
    public class Handler : IRequestHandler<UpdateTransactionReference>
    {
        private readonly ITransactionsContext _context;
        private readonly IPublishEndpoint _publishEndpoint;
        
        public Handler(ITransactionsContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<Unit> Handle(UpdateTransactionReference request, CancellationToken cancellationToken)
        {
            var transaction = await _context.Transactions.FirstAsync(x => x.Id == request.TransactionId, cancellationToken);

            if(transaction.Status != Domain.Enums.TransactionStatus.Unknown
                && transaction.Status != Domain.Enums.TransactionStatus.Unverified) 
            {
                throw new Exception("Cannot change reference.");
            }

            transaction.UpdateReference(request.Reference);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}

using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Transactions.Domain;

namespace YourBrand.Transactions.Application.Commands;

public record UpdateTransactionReference(string TransactionId, string Reference) : IRequest
{
    public class Handler(ITransactionsContext context, IPublishEndpoint publishEndpoint) : IRequestHandler<UpdateTransactionReference>
    {
        public async Task Handle(UpdateTransactionReference request, CancellationToken cancellationToken)
        {
            var transaction = await context.Transactions.FirstAsync(x => x.Id == request.TransactionId, cancellationToken);

            if (transaction.Status != Domain.Enums.TransactionStatus.Unknown
                && transaction.Status != Domain.Enums.TransactionStatus.Unverified)
            {
                throw new Exception("Cannot change reference.");
            }

            transaction.UpdateReference(request.Reference);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}
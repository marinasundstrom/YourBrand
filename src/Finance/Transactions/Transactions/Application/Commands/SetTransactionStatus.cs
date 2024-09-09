using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Transactions.Domain;
using YourBrand.Transactions.Domain.Enums;

namespace YourBrand.Transactions.Application.Commands;

public record SetTransactionStatus(string OrganizationId, string TransactionId, TransactionStatus Status) : IRequest
{
    public class Handler(ITransactionsContext context) : IRequestHandler<SetTransactionStatus>
    {
        public async Task Handle(SetTransactionStatus request, CancellationToken cancellationToken)
        {
            var transaction = await context.Transactions
                .InOrganization(request.OrganizationId)
                .FirstAsync(x => x.Id == request.TransactionId, cancellationToken);

            transaction.SetStatus(request.Status);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}
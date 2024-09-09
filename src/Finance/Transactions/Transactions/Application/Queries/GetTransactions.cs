using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Transactions.Domain;
using YourBrand.Transactions.Domain.Enums;

namespace YourBrand.Transactions.Application.Queries;

public record GetTransactions(string OrganizationId, int Page, int PageSize, TransactionStatus[]? Status = null) : IRequest<ItemsResult<TransactionDto>>
{
    public class Handler(ITransactionsContext context, IPublishEndpoint publishEndpoint) : IRequestHandler<GetTransactions, ItemsResult<TransactionDto>>
    {
        public async Task<ItemsResult<TransactionDto>> Handle(GetTransactions request, CancellationToken cancellationToken)
        {
            if (request.PageSize < 0)
            {
                throw new Exception("Page Size cannot be negative.");
            }

            if (request.PageSize > 100)
            {
                throw new Exception("Page Size must not be greater than 100.");
            }

            var query = context.Transactions
                .InOrganization(request.OrganizationId)
                .AsSplitQuery()
                .AsNoTracking()
                .OrderByDescending(x => x.Date)
                .AsQueryable();

            if (request.Status?.Any() ?? false)
            {
                var statuses = request.Status.Select(x => (int)x);
                query = query.Where(i => statuses.Any(s => s == (int)i.Status));
            }

            int totalItems = await query.CountAsync(cancellationToken);

            query = query
                .Skip(request.Page * request.PageSize)
                .Take(request.PageSize);

            var items = await query.ToArrayAsync(cancellationToken);

            return new ItemsResult<TransactionDto>(
                items.Select(t => new TransactionDto(t.OrganizationId, t.Id,  t.Date, t.Status, t.From!, t.Reference!, t.Currency, t.Amount)),
                totalItems);
        }
    }
}
using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Transactions.Domain;
using Transactions.Domain.Enums;

namespace Transactions.Application.Queries;

public record GetPayments(int Page, int PageSize, TransactionStatus[]? Status = null, int? InvoiceId = null) : IRequest<ItemsResult<TransactionDto>>
{
    public class Handler : IRequestHandler<GetPayments, ItemsResult<TransactionDto>>
    {
        private readonly ITransactionsContext _context;
        private readonly IPublishEndpoint _publishEndpoint;

        public Handler(ITransactionsContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<ItemsResult<TransactionDto>> Handle(GetPayments request, CancellationToken cancellationToken)
        {
            if(request.PageSize < 0) 
            {
                throw new Exception("Page Size cannot be negative.");
            }

            if(request.PageSize > 100) 
            {
                throw new Exception("Page Size must not be greater than 100.");
            }

            var query = _context.Transactions
                .AsSplitQuery()
                .AsNoTracking()
                .OrderByDescending(x => x.Date)
                .AsQueryable();

            if (request.Status?.Any() ?? false)
            {
                var statuses = request.Status.Select(x => (int)x);
                query = query.Where(i => statuses.Any(s => s == (int)i.Status));
            }

            if(request.InvoiceId is not null) 
            {
                int invoiceId = request.InvoiceId.GetValueOrDefault();
                query = query.Where(i => i.InvoiceId == invoiceId);
            }


            int totalItems = await query.CountAsync(cancellationToken);

            query = query
                .Skip(request.Page * request.PageSize)
                .Take(request.PageSize);

            var items = await query.ToArrayAsync(cancellationToken);

            return new ItemsResult<TransactionDto>(
                items.Select(t => new TransactionDto(t.Id, t.Date, t.Status, t.From!, t.Reference!, t.Currency, t.Amount, t.InvoiceId)),
                totalItems);
        }
    }
}
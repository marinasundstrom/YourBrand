using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Payments.Domain;
using YourBrand.Payments.Domain.Enums;

namespace YourBrand.Payments.Application.Queries;

public record GetPayments(int Page, int PageSize, PaymentStatus[]? Status = null, string? InvoiceId = null) : IRequest<ItemsResult<PaymentDto>>
{
    public class Handler(IPaymentsContext context) : IRequestHandler<GetPayments, ItemsResult<PaymentDto>>
    {
        public async Task<ItemsResult<PaymentDto>> Handle(GetPayments request, CancellationToken cancellationToken)
        {
            if (request.PageSize < 0)
            {
                throw new Exception("Page Size cannot be negative.");
            }

            if (request.PageSize > 100)
            {
                throw new Exception("Page Size must not be greater than 100.");
            }

            var query = context.Payments
                .AsSplitQuery()
                .AsNoTracking()
                .OrderByDescending(x => x.Created)
                .AsQueryable();

            if (request.Status?.Any() ?? false)
            {
                var statuses = request.Status.Select(x => (int)x);
                query = query.Where(i => statuses.Any(s => s == (int)i.Status));
            }

            if (request.InvoiceId is not null)
            {
                string invoiceId = request.InvoiceId;
                query = query.Where(i => i.InvoiceId == invoiceId);
            }


            int totalItems = await query.CountAsync(cancellationToken);

            query = query
                .Skip(request.Page * request.PageSize)
                .Take(request.PageSize);

            var items = await query.ToArrayAsync(cancellationToken);

            return new ItemsResult<PaymentDto>(
                items.Select(t => t.ToDto()),
                totalItems);
        }
    }
}
using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Payments.Domain;
using YourBrand.Payments.Domain.Enums;

namespace YourBrand.Payments.Application.Queries;

public record GetPayments(int Page, int PageSize, PaymentStatus[]? Status = null, int? InvoiceId = null) : IRequest<ItemsResult<PaymentDto>>
{
    public class Handler : IRequestHandler<GetPayments, ItemsResult<PaymentDto>>
    {
        private readonly IPaymentsContext _context;
        private readonly IPublishEndpoint _publishEndpoint;

        public Handler(IPaymentsContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<ItemsResult<PaymentDto>> Handle(GetPayments request, CancellationToken cancellationToken)
        {
            if(request.PageSize < 0) 
            {
                throw new Exception("Page Size cannot be negative.");
            }

            if(request.PageSize > 100) 
            {
                throw new Exception("Page Size must not be greater than 100.");
            }

            var query = _context.Payments
                .AsSplitQuery()
                .AsNoTracking()
                .OrderByDescending(x => x.Created)
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

            return new ItemsResult<PaymentDto>(
                items.Select(t => new PaymentDto(t.Id, t.InvoiceId, t.Status, t.Currency, t.Amount, t.DueDate, t.PaymentMethod)),
                totalItems);
        }
    }
}
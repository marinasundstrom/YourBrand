namespace YourBrand.RotRutService.Application.Queries;

/*
public record GetInvoice(int InvoiceId) : IRequest<InvoiceDto?>
{
    public class Handler : IRequestHandler<GetInvoice, InvoiceDto?>
    {
        private readonly IRotRutContext _context;

        public Handler(IRotRutContext context)
        {
            _context = context;
        }

        public async Task<InvoiceDto?> Handle(GetInvoice request, CancellationToken cancellationToken)
        {
            var invoice = await _context.RotRutService
                .Include(i => i.Items)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.InvoiceId, cancellationToken);

            return invoice is null
                ? null
                : invoice.ToDto();
        }
    }
}
*/
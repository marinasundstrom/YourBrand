using YourBrand.Warehouse.Domain;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Warehouse.Application.Items.Commands;

public record DeleteItem(string ItemId) : IRequest
{
    public class Handler : IRequestHandler<DeleteItem>
    {
        private readonly IWarehouseContext _context;

        public Handler(IWarehouseContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteItem request, CancellationToken cancellationToken)
        {
            /*
            var invoice = await _context.Items
                //.Include(i => i.Addresses)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.ItemId, cancellationToken);

            if(invoice is null)
            {
                throw new Exception();
            }

            _context.Items.Remove(invoice);

            await _context.SaveChangesAsync(cancellationToken);
            */

            return Unit.Value;
        }
    }
}
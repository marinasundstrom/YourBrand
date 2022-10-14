
using YourBrand.Inventory.Domain;

using MediatR;
using Microsoft.EntityFrameworkCore;

namespace YourBrand.Inventory.Application.Items.Commands;

public record PickItems(string Id, int Quantity, bool FromReserved = false) : IRequest
{
    public class Handler : IRequestHandler<PickItems>
    {
        private readonly IInventoryContext _context;

        public Handler(IInventoryContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(PickItems request, CancellationToken cancellationToken)
        {
            var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (item is null) throw new Exception();

            item.Pick(request.Quantity, request.FromReserved);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}

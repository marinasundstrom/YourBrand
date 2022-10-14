
using YourBrand.Inventory.Domain;

using MediatR;
using Microsoft.EntityFrameworkCore;

namespace YourBrand.Inventory.Application.Items.Commands;

public record ShipItems(string Id, int Quantity, bool FromPicked = false) : IRequest
{
    public class Handler : IRequestHandler<ShipItems>
    {
        private readonly IInventoryContext _context;

        public Handler(IInventoryContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(ShipItems request, CancellationToken cancellationToken)
        {
            var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (item is null) throw new Exception();

            item.Ship(request.Quantity, request.FromPicked);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}

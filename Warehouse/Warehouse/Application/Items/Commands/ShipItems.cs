
using YourBrand.Warehouse.Domain;

using MediatR;
using Microsoft.EntityFrameworkCore;

namespace YourBrand.Warehouse.Application.Items.Commands;

public record ShipItems(string Id, int Quantity, bool FromPicked = false) : IRequest
{
    public class Handler : IRequestHandler<ShipItems>
    {
        private readonly IWarehouseContext _context;

        public Handler(IWarehouseContext context)
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

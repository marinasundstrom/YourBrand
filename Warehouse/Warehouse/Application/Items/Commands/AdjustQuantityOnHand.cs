
using YourBrand.Warehouse.Domain;

using MediatR;
using Microsoft.EntityFrameworkCore;

namespace YourBrand.Warehouse.Application.Items.Commands;

public record AdjustQuantityOnHand(string Id, int NewQuantityOnHand) : IRequest
{
    public class Handler : IRequestHandler<AdjustQuantityOnHand>
    {
        private readonly IWarehouseContext _context;

        public Handler(IWarehouseContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(AdjustQuantityOnHand request, CancellationToken cancellationToken)
        {
            var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (item is null) throw new Exception();

            item.AdjustQuantityOnHand(request.NewQuantityOnHand);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}

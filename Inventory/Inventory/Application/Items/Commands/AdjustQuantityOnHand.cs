
using YourBrand.Inventory.Domain;

using MediatR;
using Microsoft.EntityFrameworkCore;

namespace YourBrand.Inventory.Application.Items.Commands;

public record AdjustQuantityOnHand(string Id, int NewQuantityOnHand) : IRequest
{
    public class Handler : IRequestHandler<AdjustQuantityOnHand>
    {
        private readonly IInventoryContext _context;

        public Handler(IInventoryContext context)
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

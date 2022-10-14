
using YourBrand.Inventory.Domain;

using MediatR;
using Microsoft.EntityFrameworkCore;

namespace YourBrand.Inventory.Application.Items.Commands;

public record ReceiveItems(string Id, int Quantity) : IRequest
{
    public class Handler : IRequestHandler<ReceiveItems>
    {
        private readonly IInventoryContext _context;

        public Handler(IInventoryContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(ReceiveItems request, CancellationToken cancellationToken)
        {
            var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (item is null) throw new Exception();

            item.Receive(request.Quantity);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}

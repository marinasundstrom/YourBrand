
using YourBrand.Inventory.Domain;

using MediatR;
using YourBrand.Inventory.Application.Items;
using Microsoft.EntityFrameworkCore;

namespace YourBrand.Inventory.Application.Items.Commands;

public record UpdateItem(string Id, string Name, string Unit) : IRequest
{
    public class Handler : IRequestHandler<UpdateItem>
    {
        private readonly IInventoryContext _context;

        public Handler(IInventoryContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateItem request, CancellationToken cancellationToken)
        {
            var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (item is null) throw new Exception();

            item.Name = request.Name;
            item.Unit = request.Unit;

            await _context.SaveChangesAsync(cancellationToken);

            return MediatR.Unit.Value;
        }
    }
}

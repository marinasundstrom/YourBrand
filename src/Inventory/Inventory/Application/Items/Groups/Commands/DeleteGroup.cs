using MediatR;

using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Items.Groups.Commands;

public record DeleteItemGroup(string ItemGroupId) : IRequest
{
    public class Handler(IInventoryContext context) : IRequestHandler<DeleteItemGroup>
    {
        public async Task Handle(DeleteItemGroup request, CancellationToken cancellationToken)
        {
            /*
            var invoice = await _context.ItemGroups
                //.Include(i => i.Addresses)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.ItemGroupId, cancellationToken);

            if(invoice is null)
            {
                throw new Exception();
            }

            _context.ItemGroups.Remove(invoice);

            await _context.SaveChangesAsync(cancellationToken);
            */

        }
    }
}
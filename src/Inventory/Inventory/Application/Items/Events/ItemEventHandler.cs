using YourBrand.Domain;
using YourBrand.Inventory.Application.Common.Interfaces;
using YourBrand.Inventory.Domain;
using YourBrand.Inventory.Domain.Events;

namespace YourBrand.Inventory.Application.Items.Events;

public class ItemEventHandler(IInventoryContext context)
        : IDomainEventHandler<ItemCreated>
{
    public async Task Handle(ItemCreated notification, CancellationToken cancellationToken)
    {
        /*
        var person = await _context.Items
            .FirstOrDefaultAsync(i => i.Id == notification.ItemId);

        if(person is not null) 
        {
           
        }
        */
    }
}
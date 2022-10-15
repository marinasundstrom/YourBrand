using YourBrand.Inventory.Application.Common.Models;
using YourBrand.Inventory.Domain;
using YourBrand.Inventory.Domain.Events;

using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Inventory.Application.Common.Interfaces;

namespace YourBrand.Inventory.Application.Items.Events;

public class ItemEventHandler 
    : IDomainEventHandler<ItemCreated>  
{
    private readonly IInventoryContext _context;

    public ItemEventHandler(IInventoryContext context)
    {
        _context = context;
    }

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
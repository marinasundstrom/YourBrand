using YourBrand.Inventory.Application.Common.Models;
using YourBrand.Inventory.Domain;
using YourBrand.Inventory.Domain.Events;

using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Inventory.Application.Common.Interfaces;

namespace YourBrand.Inventory.Application.Items.Events;

public class ItemCreatedHandler 
: IDomainEventHandler<ItemCreated>, 
  IDomainEventHandler<ItemQuantityOnHandUpdated>, 
  IDomainEventHandler<ItemsPicked>,
  IDomainEventHandler<ItemsReserved>,
  IDomainEventHandler<ItemQuantityAvailableUpdated>
{
    private readonly IInventoryContext _context;

    public ItemCreatedHandler(IInventoryContext context)
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

    public async Task Handle(ItemQuantityOnHandUpdated notification, CancellationToken cancellationToken)
    {

    }

    public async Task Handle(ItemsPicked notification, CancellationToken cancellationToken)
    {
    }

    public async Task Handle(ItemsReserved notification, CancellationToken cancellationToken)
    {
    }

    public async Task Handle(ItemQuantityAvailableUpdated notification, CancellationToken cancellationToken)
    {
    }
}
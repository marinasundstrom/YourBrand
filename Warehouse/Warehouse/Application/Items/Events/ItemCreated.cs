using YourBrand.Warehouse.Application.Common.Models;
using YourBrand.Warehouse.Domain;
using YourBrand.Warehouse.Domain.Events;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Warehouse.Application.Items.Events;

public class ItemCreatedHandler : INotificationHandler<DomainEventNotification<ItemCreated>>
{
    private readonly IWarehouseContext _context;

    public ItemCreatedHandler(IWarehouseContext context)
    {
        _context = context;
    }

    public async Task Handle(DomainEventNotification<ItemCreated> notification, CancellationToken cancellationToken)
    {
        /*
        var person = await _context.Items
            .FirstOrDefaultAsync(i => i.Id == notification.DomainEvent.ItemId);

        if(person is not null) 
        {
           
        }
        */
    }
}
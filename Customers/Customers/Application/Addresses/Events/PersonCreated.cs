using YourBrand.Customers.Application.Common.Models;
using YourBrand.Customers.Domain;
using YourBrand.Customers.Domain.Events;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Customers.Application.Addresses.Events;

public class AddressCreatedHandler : INotificationHandler<DomainEventNotification<AddressCreated>>
{
    private readonly ICustomersContext _context;

    public AddressCreatedHandler(ICustomersContext context)
    {
        _context = context;
    }

    public async Task Handle(DomainEventNotification<AddressCreated> notification, CancellationToken cancellationToken)
    {
        var person = await _context.Addresses
            .FirstOrDefaultAsync(i => i.Id == notification.DomainEvent.AddressId);

        if(person is not null) 
        {
           
        }
    }
}
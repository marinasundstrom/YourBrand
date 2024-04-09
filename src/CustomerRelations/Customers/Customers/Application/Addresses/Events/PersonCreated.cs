using Microsoft.EntityFrameworkCore;

using YourBrand.Customers.Application.Common.Interfaces;
using YourBrand.Customers.Domain;
using YourBrand.Customers.Domain.Events;

namespace YourBrand.Customers.Application.Addresses.Events;

public class AddressCreatedHandler(ICustomersContext context) : IDomainEventHandler<AddressCreated>
{
    public async Task Handle(AddressCreated notification, CancellationToken cancellationToken)
    {
        var person = await context.Addresses
            .FirstOrDefaultAsync(i => i.Id == notification.AddressId);

        if (person is not null)
        {

        }
    }
}
using Microsoft.EntityFrameworkCore;

using YourBrand.Marketing.Application.Common.Interfaces;
using YourBrand.Marketing.Domain;
using YourBrand.Marketing.Domain.Events;

namespace YourBrand.Marketing.Application.Addresses.Events;

public class AddressCreatedHandler(IMarketingContext context) : IDomainEventHandler<AddressCreated>
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
using Microsoft.EntityFrameworkCore;

using YourBrand.Marketing.Application.Common.Interfaces;
using YourBrand.Marketing.Domain;
using YourBrand.Marketing.Domain.Events;

namespace YourBrand.Marketing.Application.Addresses.Events;

public class AddressCreatedHandler : IDomainEventHandler<AddressCreated>
{
    private readonly IMarketingContext _context;

    public AddressCreatedHandler(IMarketingContext context)
    {
        _context = context;
    }

    public async Task Handle(AddressCreated notification, CancellationToken cancellationToken)
    {
        var person = await _context.Addresses
            .FirstOrDefaultAsync(i => i.Id == notification.AddressId);

        if (person is not null)
        {

        }
    }
}
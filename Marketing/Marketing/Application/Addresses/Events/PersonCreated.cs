using YourBrand.Marketing.Application.Common.Models;
using YourBrand.Marketing.Domain;
using YourBrand.Marketing.Domain.Events;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Marketing.Application.Addresses.Events;

public class AddressCreatedHandler : INotificationHandler<DomainEventNotification<AddressCreated>>
{
    private readonly IMarketingContext _context;

    public AddressCreatedHandler(IMarketingContext context)
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
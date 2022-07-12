using YourBrand.Marketing.Application.Common.Models;
using YourBrand.Marketing.Domain;
using YourBrand.Marketing.Domain.Events;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Marketing.Application.Contacts.Events;

public class ContactCreatedHandler : INotificationHandler<DomainEventNotification<ContactCreated>>
{
    private readonly IMarketingContext _context;

    public ContactCreatedHandler(IMarketingContext context)
    {
        _context = context;
    }

    public async Task Handle(DomainEventNotification<ContactCreated> notification, CancellationToken cancellationToken)
    {
        /*
        var person = await _context.Contacts
            .FirstOrDefaultAsync(i => i.Id == notification.DomainEvent.ContactId);

        if(person is not null) 
        {
           
        }
        */
    }
}
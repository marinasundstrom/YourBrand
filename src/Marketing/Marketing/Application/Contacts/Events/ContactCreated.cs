using YourBrand.Marketing.Application.Common.Interfaces;
using YourBrand.Marketing.Domain;
using YourBrand.Marketing.Domain.Events;

namespace YourBrand.Marketing.Application.Contacts.Events;

public class ContactCreatedHandler : IDomainEventHandler<ContactCreated>
{
    private readonly IMarketingContext _context;

    public ContactCreatedHandler(IMarketingContext context)
    {
        _context = context;
    }

    public async Task Handle(ContactCreated notification, CancellationToken cancellationToken)
    {
        /*
        var person = await _context.Contacts
            .FirstOrDefaultAsync(i => i.Id == notification.ContactId);

        if(person is not null) 
        {
           
        }
        */
    }
}
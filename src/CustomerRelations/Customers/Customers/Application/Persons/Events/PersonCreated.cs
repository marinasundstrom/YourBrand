using YourBrand.Customers.Application.Common.Interfaces;
using YourBrand.Customers.Domain;
using YourBrand.Customers.Domain.Events;
using YourBrand.Domain;

namespace YourBrand.Customers.Application.Persons.Events;

public class PersonCreatedHandler(ICustomersContext context) : IDomainEventHandler<PersonCreated>
{
    public async Task Handle(PersonCreated notification, CancellationToken cancellationToken)
    {
        /*
        var person = await _context.Persons
            .FirstOrDefaultAsync(i => i.Id == notification.PersonId);

        if(person is not null) 
        {
           
        }
        */
    }
}
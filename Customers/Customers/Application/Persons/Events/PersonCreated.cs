using YourBrand.Customers.Application.Common.Models;
using YourBrand.Customers.Domain;
using YourBrand.Customers.Domain.Events;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Customers.Application.Persons.Events;

public class PersonCreatedHandler : INotificationHandler<DomainEventNotification<PersonCreated>>
{
    private readonly ICustomersContext _context;

    public PersonCreatedHandler(ICustomersContext context)
    {
        _context = context;
    }

    public async Task Handle(DomainEventNotification<PersonCreated> notification, CancellationToken cancellationToken)
    {
        /*
        var person = await _context.Persons
            .FirstOrDefaultAsync(i => i.Id == notification.DomainEvent.PersonId);

        if(person is not null) 
        {
           
        }
        */
    }
}
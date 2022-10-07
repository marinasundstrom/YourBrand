using YourBrand.Customers.Application.Common.Models;
using YourBrand.Customers.Domain;
using YourBrand.Customers.Domain.Events;

using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Customers.Application.Common.Interfaces;

namespace YourBrand.Customers.Application.Persons.Events;

public class PersonCreatedHandler : IDomainEventHandler<PersonCreated>
{
    private readonly ICustomersContext _context;

    public PersonCreatedHandler(ICustomersContext context)
    {
        _context = context;
    }

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
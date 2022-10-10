using YourBrand.Customers.Application.Common.Models;
using YourBrand.Customers.Domain;
using YourBrand.Customers.Domain.Events;

using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Customers.Application.Common.Interfaces;

namespace YourBrand.Customers.Application.Organizations.Events;

public class OrganizationCreatedHandler : IDomainEventHandler<OrganizationCreated>
{
    private readonly ICustomersContext _context;

    public OrganizationCreatedHandler(ICustomersContext context)
    {
        _context = context;
    }

    public async Task Handle(OrganizationCreated notification, CancellationToken cancellationToken)
    {
        /*
        var organization = await _context.Organizations
            .FirstOrDefaultAsync(i => i.Id == notification.OrganizationId);

        if(organization is not null) 
        {
           
        }
        */
    }
}
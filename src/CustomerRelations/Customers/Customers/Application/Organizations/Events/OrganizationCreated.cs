using YourBrand.Customers.Application.Common.Interfaces;
using YourBrand.Customers.Domain;
using YourBrand.Customers.Domain.Events;

namespace YourBrand.Customers.Application.Organizations.Events;

public class OrganizationCreatedHandler(ICustomersContext context) : IDomainEventHandler<OrganizationCreated>
{
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
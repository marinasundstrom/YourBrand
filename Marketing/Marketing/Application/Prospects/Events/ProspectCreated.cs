using YourBrand.Marketing.Application.Common.Models;
using YourBrand.Marketing.Domain;
using YourBrand.Marketing.Domain.Events;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Marketing.Application.Prospects.Events;

public class ProspectCreatedHandler : INotificationHandler<DomainEventNotification<ProspectCreated>>
{
    private readonly IMarketingContext _context;

    public ProspectCreatedHandler(IMarketingContext context)
    {
        _context = context;
    }

    public async Task Handle(DomainEventNotification<ProspectCreated> notification, CancellationToken cancellationToken)
    {
        /*
        var person = await _context.Prospects
            .FirstOrDefaultAsync(i => i.Id == notification.DomainEvent.ProspectId);

        if(person is not null) 
        {
           
        }
        */
    }
}
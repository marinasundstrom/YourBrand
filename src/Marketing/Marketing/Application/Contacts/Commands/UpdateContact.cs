
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Marketing.Domain;

namespace YourBrand.Marketing.Application.Contacts.Commands;

public record UpdateContact(string Id, string FirstName, string LastName, string SSN, string CampaignId) : IRequest<ContactDto>
{
    public class Handler(IMarketingContext context) : IRequestHandler<UpdateContact, ContactDto>
    {
        public async Task<ContactDto> Handle(UpdateContact request, CancellationToken cancellationToken)
        {
            var contact = await context.Contacts
                .Include(i => i.Campaign)
                .FirstAsync(x => x.Id == request.Id, cancellationToken);

            contact.FirstName = request.FirstName;
            contact.LastName = request.LastName;
            contact.Ssn = request.SSN;
            contact.Campaign = await context.Campaigns.FirstOrDefaultAsync(x => x.Id == request.CampaignId, cancellationToken);

            await context.SaveChangesAsync(cancellationToken);

            return contact.ToDto();
        }
    }
}
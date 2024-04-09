
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Marketing.Domain;

namespace YourBrand.Marketing.Application.Contacts.Commands;

public record CreateContact(string FirstName, string LastName, string SSN, string CampaignId) : IRequest<ContactDto>
{
    public class Handler(IMarketingContext context) : IRequestHandler<CreateContact, ContactDto>
    {
        public async Task<ContactDto> Handle(CreateContact request, CancellationToken cancellationToken)
        {
            var contact = new Domain.Entities.Contact(request.FirstName, request.LastName, request.SSN);
            contact.Campaign = await context.Campaigns.FirstOrDefaultAsync(x => x.Id == request.CampaignId, cancellationToken);

            context.Contacts.Add(contact);

            await context.SaveChangesAsync(cancellationToken);

            return contact.ToDto();
        }
    }
}
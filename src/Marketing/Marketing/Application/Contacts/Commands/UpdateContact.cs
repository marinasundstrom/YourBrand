
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Marketing.Domain;

namespace YourBrand.Marketing.Application.Contacts.Commands;

public record UpdateContact(string Id, string FirstName, string LastName, string SSN, string CampaignId) : IRequest<ContactDto>
{
    public class Handler : IRequestHandler<UpdateContact, ContactDto>
    {
        private readonly IMarketingContext _context;

        public Handler(IMarketingContext context)
        {
            _context = context;
        }

        public async Task<ContactDto> Handle(UpdateContact request, CancellationToken cancellationToken)
        {
            var contact = await _context.Contacts
                .Include(i => i.Campaign)
                .FirstAsync(x => x.Id == request.Id, cancellationToken);

            contact.FirstName = request.FirstName;
            contact.LastName = request.LastName;
            contact.Ssn = request.SSN;
            contact.Campaign = await _context.Campaigns.FirstOrDefaultAsync(x => x.Id == request.CampaignId, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return contact.ToDto();
        }
    }
}

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Marketing.Domain;

namespace YourBrand.Marketing.Application.Contacts.Commands;

public record CreateContact(string FirstName, string LastName, string SSN, string CampaignId) : IRequest<ContactDto>
{
    public class Handler : IRequestHandler<CreateContact, ContactDto>
    {
        private readonly IMarketingContext _context;

        public Handler(IMarketingContext context)
        {
            _context = context;
        }

        public async Task<ContactDto> Handle(CreateContact request, CancellationToken cancellationToken)
        {
            var contact = new Domain.Entities.Contact(request.FirstName, request.LastName, request.SSN);
            contact.Campaign = await _context.Campaigns.FirstOrDefaultAsync(x => x.Id == request.CampaignId, cancellationToken);

            _context.Contacts.Add(contact);

            await _context.SaveChangesAsync(cancellationToken);

            return contact.ToDto();
        }
    }
}
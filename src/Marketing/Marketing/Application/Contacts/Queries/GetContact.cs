using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Marketing.Domain;

namespace YourBrand.Marketing.Application.Contacts.Queries;

public record GetContact(string ContactId) : IRequest<ContactDto?>
{
    public class Handler(IMarketingContext context) : IRequestHandler<GetContact, ContactDto?>
    {
        public async Task<ContactDto?> Handle(GetContact request, CancellationToken cancellationToken)
        {
            var person = await context.Contacts
                .Include(i => i.Campaign)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.ContactId, cancellationToken);

            return person?.ToDto();
        }
    }
}
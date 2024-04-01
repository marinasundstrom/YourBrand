using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Marketing.Domain;

namespace YourBrand.Marketing.Application.Contacts.Queries;

public record GetContact(string ContactId) : IRequest<ContactDto?>
{
    public class Handler : IRequestHandler<GetContact, ContactDto?>
    {
        private readonly IMarketingContext _context;

        public Handler(IMarketingContext context)
        {
            _context = context;
        }

        public async Task<ContactDto?> Handle(GetContact request, CancellationToken cancellationToken)
        {
            var person = await _context.Contacts
                .Include(i => i.Campaign)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.ContactId, cancellationToken);

            return person?.ToDto();
        }
    }
}
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Marketing.Domain;

namespace YourBrand.Marketing.Application.Contacts.Commands;

public record DeleteContact(string Id) : IRequest
{
    public class Handler : IRequestHandler<DeleteContact>
    {
        private readonly IMarketingContext _context;

        public Handler(IMarketingContext context)
        {
            _context = context;
        }
        public async Task Handle(DeleteContact request, CancellationToken cancellationToken)
        {
            var contact = await _context.Contacts
                .Include(i => i.Campaign)
                .FirstAsync(x => x.Id == request.Id, cancellationToken);

            _context.Contacts.Remove(contact);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
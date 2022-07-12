using YourBrand.Marketing.Domain;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Marketing.Application.Contacts.Commands;

public record DeleteContact(string ContactId) : IRequest
{
    public class Handler : IRequestHandler<DeleteContact>
    {
        private readonly IMarketingContext _context;

        public Handler(IMarketingContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteContact request, CancellationToken cancellationToken)
        {
            /*
            var invoice = await _context.Contacts
                //.Include(i => i.Addresses)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.ContactId, cancellationToken);

            if(invoice is null)
            {
                throw new Exception();
            }

            _context.Contacts.Remove(invoice);

            await _context.SaveChangesAsync(cancellationToken);
            */

            return Unit.Value;
        }
    }
}
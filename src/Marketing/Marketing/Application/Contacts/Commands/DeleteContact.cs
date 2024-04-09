using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Marketing.Domain;

namespace YourBrand.Marketing.Application.Contacts.Commands;

public record DeleteContact(string Id) : IRequest
{
    public class Handler(IMarketingContext context) : IRequestHandler<DeleteContact>
    {
        public async Task Handle(DeleteContact request, CancellationToken cancellationToken)
        {
            var contact = await context.Contacts
                .Include(i => i.Campaign)
                .FirstAsync(x => x.Id == request.Id, cancellationToken);

            context.Contacts.Remove(contact);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
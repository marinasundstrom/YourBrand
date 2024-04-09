using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Sites.Commands;

public record DeleteSiteCommand(string Id) : IRequest
{
    public class DeleteSiteCommandHandler(IInventoryContext context) : IRequestHandler<DeleteSiteCommand>
    {
        public async Task Handle(DeleteSiteCommand request, CancellationToken cancellationToken)
        {
            var site = await context.Sites
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (site is null) throw new Exception();

            context.Sites.Remove(site);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}
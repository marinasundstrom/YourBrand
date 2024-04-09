using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Sites.Commands;

public record UpdateSiteCommand(string Id, string Name) : IRequest
{
    public class UpdateSiteCommandHandler(IInventoryContext context) : IRequestHandler<UpdateSiteCommand>
    {
        public async Task Handle(UpdateSiteCommand request, CancellationToken cancellationToken)
        {
            var site = await context.Sites.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (site is null) throw new Exception();

            site.Name = request.Name;

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}
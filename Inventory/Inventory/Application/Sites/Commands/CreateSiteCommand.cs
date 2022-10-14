using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Sites.Commands;

public record CreateSiteCommand(string Name) : IRequest<SiteDto>
{
    public class CreateSiteCommandHandler : IRequestHandler<CreateSiteCommand, SiteDto>
    {
        private readonly IInventoryContext context;

        public CreateSiteCommandHandler(IInventoryContext context)
        {
            this.context = context;
        }

        public async Task<SiteDto> Handle(CreateSiteCommand request, CancellationToken cancellationToken)
        {
            var site = await context.Sites.FirstOrDefaultAsync(i => i.Name == request.Name, cancellationToken);

            if (site is not null) throw new Exception();

            site = new Domain.Entities.Site(Guid.NewGuid().ToString(), request.Name);

            context.Sites.Add(site);

            await context.SaveChangesAsync(cancellationToken);

            return site.ToDto();
        }
    }
}

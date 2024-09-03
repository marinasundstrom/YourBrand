using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.Brands.Commands;

public sealed record DeleteBrandCommand(string OrganizationId, int Id) : IRequest
{
    public sealed class DeleteBrandCommandHandler(CatalogContext context) : IRequestHandler<DeleteBrandCommand>
    {
        public async Task Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
        {
            var brand = await context.Brands
               .InOrganization(request.OrganizationId)
               .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (brand is null) throw new Exception();

            context.Brands.Remove(brand);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
﻿using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.Brands.Commands;

public sealed record UpdateBrandCommand(string OrganizationId, int Id, string Name, string Handle) : IRequest
{
    public sealed class UpdateBrandCommandHandler(CatalogContext context) : IRequestHandler<UpdateBrandCommand>
    {
        public async Task Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
        {
            var brand = await context.Brands
               .InOrganization(request.OrganizationId)
               .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (brand is null) throw new Exception();

            brand.Name = request.Name;

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
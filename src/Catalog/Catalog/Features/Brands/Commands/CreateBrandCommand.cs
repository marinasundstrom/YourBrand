using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;
namespace YourBrand.Catalog.Features.Brands.Commands;

public sealed record CreateBrandCommand(string Name, string Handle) : IRequest<BrandDto>
{
    public sealed class CreateBrandCommandHandler(CatalogContext context) : IRequestHandler<CreateBrandCommand, BrandDto>
    {
        public async Task<BrandDto> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
        {
            var brand = await context.Brands.FirstOrDefaultAsync(i => i.Name == request.Name, cancellationToken);

            if (brand is not null) throw new Exception();

            if (await context.Brands.AnyAsync(x => x.Name == request.Name))
            {
                throw new Exception("Brand with name already exists");
            }

            if (await context.Brands.AnyAsync(x => x.Handle == request.Handle))
            {
                throw new Exception("Handle already in use");
            }

            brand = new Domain.Entities.Brand(request.Name, request.Handle);

            context.Brands.Add(brand);

            await context.SaveChangesAsync(cancellationToken);

            brand = await context
               .Brands
               .AsNoTracking()
               .FirstAsync(c => c.Id == brand.Id);

            return brand.ToDto();
        }
    }
}
using YourBrand.Catalog.Persistence;

using MediatR;

using Microsoft.EntityFrameworkCore;
namespace YourBrand.Catalog.Features.Brands.Commands;

public sealed record CreateBrandCommand(string Name, string Handle) : IRequest<BrandDto>
{
    public sealed class CreateBrandCommandHandler : IRequestHandler<CreateBrandCommand, BrandDto>
    {
        private readonly CatalogContext context;

        public CreateBrandCommandHandler(CatalogContext context)
        {
            this.context = context;
        }

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
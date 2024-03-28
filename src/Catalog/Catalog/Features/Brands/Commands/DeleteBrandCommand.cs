using YourBrand.Catalog.Persistence;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Catalog.Features.Brands.Commands;

public sealed record DeleteBrandCommand(int Id) : IRequest
{
    public sealed class DeleteBrandCommandHandler : IRequestHandler<DeleteBrandCommand>
    {
        private readonly CatalogContext context;

        public DeleteBrandCommandHandler(CatalogContext context)
        {
            this.context = context;
        }

        public async Task Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
        {
            var brand = await context.Brands
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (brand is null) throw new Exception();

            context.Brands.Remove(brand);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
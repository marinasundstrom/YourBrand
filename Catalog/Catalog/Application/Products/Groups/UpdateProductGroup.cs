using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain;
using YourBrand.Catalog.Domain.Entities;

namespace YourBrand.Catalog.Application.Products.Groups;

public record UpdateProductGroup(string ProductGroupId, ApiUpdateProductGroup Data) : IRequest<ProductGroupDto>
{
    public class Handler : IRequestHandler<UpdateProductGroup, ProductGroupDto>
    {
        private readonly ICatalogContext _context;

        public Handler(ICatalogContext context)
        {
            _context = context;
        }

        public async Task<ProductGroupDto> Handle(UpdateProductGroup request, CancellationToken cancellationToken)
        {
            var productGroup = await _context.ProductGroups
                    .FirstAsync(x => x.Id == request.ProductGroupId);

            var parentGroup = await _context.ProductGroups
                .FirstOrDefaultAsync(x => x.Id == request.Data.ParentGroupId);

            productGroup.Name = request.Data.Name;
            productGroup.Description = request.Data.Description;
            productGroup.Parent = parentGroup;

            await _context.SaveChangesAsync();

            return new ProductGroupDto(productGroup.Id, productGroup.Name, productGroup.Description, productGroup?.Parent?.Id);
        }
    }
}

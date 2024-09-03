using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Products.Attributes;

public record GetProductAttributes(string OrganizationId, long ProductId) : IRequest<IEnumerable<ProductAttributeDto>>
{
    public class Handler(CatalogContext context) : IRequestHandler<GetProductAttributes, IEnumerable<ProductAttributeDto>>
    {
        public async Task<IEnumerable<ProductAttributeDto>> Handle(GetProductAttributes request, CancellationToken cancellationToken)
        {
            var product = await context.Products
                .InOrganization(request.OrganizationId)
                .AsSplitQuery()
                .AsNoTracking()
                .Include(pv => pv.ProductAttributes)
                .ThenInclude(pv => pv.Value)
                .Include(pv => pv.ProductAttributes)
                .ThenInclude(pv => pv.Attribute)
                .ThenInclude(pv => pv.Group)
                .Include(pv => pv.ProductAttributes)
                .ThenInclude(pv => pv.Attribute)
                .ThenInclude(pv => pv.Values)
                .FirstAsync(p => p.Id == request.ProductId, cancellationToken);

            return product.ProductAttributes.Select(x => x.ToDto());
        }
    }
}
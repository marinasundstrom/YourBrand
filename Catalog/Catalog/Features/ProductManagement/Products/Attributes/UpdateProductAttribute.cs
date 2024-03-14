using YourBrand.Catalog.Persistence;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Catalog.Features.ProductManagement.Products.Attributes;

public record UpdateProductAttribute(long ProductId, string AttributeId, string ValueId, bool ForVariant, bool IsMainAttribute) : IRequest<ProductAttributeDto>
{
    public class Handler : IRequestHandler<UpdateProductAttribute, ProductAttributeDto>
    {
        private readonly CatalogContext _context;

        public Handler(CatalogContext context)
        {
            _context = context;
        }

        public async Task<ProductAttributeDto> Handle(UpdateProductAttribute request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                            .AsSplitQuery()
                            .Include(x => x.ProductAttributes)
                            .ThenInclude(x => x.Attribute)
                            .ThenInclude(x => x.Values)
                            .FirstAsync(attribute => attribute.Id == request.ProductId, cancellationToken);

            var productAttribute = product.ProductAttributes
                .First(attribute => attribute.AttributeId == request.AttributeId);

            var value = productAttribute.Value?.Id == request.ValueId ? productAttribute.Value : productAttribute.Attribute.Values
                .First(x => x.Id == request.ValueId);

            productAttribute.ProductId = product.Id;
            productAttribute.Value = value;
            productAttribute.ForVariant = request.ForVariant;
            productAttribute.IsMainAttribute = request.IsMainAttribute;

            product.AddProductAttribute(productAttribute);

            await _context.SaveChangesAsync(cancellationToken);

            return productAttribute.ToDto();
        }
    }
}
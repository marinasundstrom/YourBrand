using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Products.Attributes;

public record UpdateProductAttribute(long ProductId, string AttributeId, string ValueId, bool ForVariant, bool IsMainAttribute) : IRequest<ProductAttributeDto>
{
    public class Handler(CatalogContext context) : IRequestHandler<UpdateProductAttribute, ProductAttributeDto>
    {
        public async Task<ProductAttributeDto> Handle(UpdateProductAttribute request, CancellationToken cancellationToken)
        {
            var product = await context.Products
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

            await context.SaveChangesAsync(cancellationToken);

            return productAttribute.ToDto();
        }
    }
}
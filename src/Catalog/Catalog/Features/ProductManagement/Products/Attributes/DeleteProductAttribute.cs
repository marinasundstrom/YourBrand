using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Products.Attributes;

public record DeleteProductAttribute(string OrganizationId, int ProductId, string AttributeId) : IRequest
{
    public class Handler(CatalogContext context) : IRequestHandler<DeleteProductAttribute>
    {
        public async Task Handle(DeleteProductAttribute request, CancellationToken cancellationToken)
        {
            var product = await context.Products
                .InOrganization(request.OrganizationId)
                .Include(x => x.ProductAttributes)
                .FirstAsync(x => x.Id == request.ProductId);

            var attribute = product.ProductAttributes
                .First(x => x.AttributeId == request.AttributeId);

            product.RemoveProductAttribute(attribute);
            context.ProductAttributes.Remove(attribute);

            await context.SaveChangesAsync();

        }
    }
}
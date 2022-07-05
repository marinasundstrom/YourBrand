using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Products.Application.Attributes;
using YourBrand.Products.Domain;
using YourBrand.Products.Domain.Entities;

namespace YourBrand.Products.Application.Products.Attributes.Groups;

public record UpdateProductAttributeGroup(string ProductId, string AttributeGroupId, ApiUpdateProductAttributeGroup Data) : IRequest<AttributeGroupDto>
{
    public class Handler : IRequestHandler<UpdateProductAttributeGroup, AttributeGroupDto>
    {
        private readonly IProductsContext _context;

        public Handler(IProductsContext context)
        {
            _context = context;
        }

        public async Task<AttributeGroupDto> Handle(UpdateProductAttributeGroup request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
            .Include(x => x.AttributeGroups)
            .FirstAsync(x => x.Id == request.ProductId);

            var attributeGroup = product.AttributeGroups
                .First(x => x.Id == request.AttributeGroupId);

            attributeGroup.Name = request.Data.Name;
            attributeGroup.Description = request.Data.Description;

            await _context.SaveChangesAsync();

            return new AttributeGroupDto(attributeGroup.Id, attributeGroup.Name, attributeGroup.Description);
        }
    }
}

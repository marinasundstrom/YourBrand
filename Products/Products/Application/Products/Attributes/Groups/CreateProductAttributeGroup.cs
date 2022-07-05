using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Products.Application.Attributes;
using YourBrand.Products.Domain;
using YourBrand.Products.Domain.Entities;

namespace YourBrand.Products.Application.Products.Attributes.Groups;

public record CreateProductAttributeGroup(string ProductId, ApiCreateProductAttributeGroup Data) : IRequest<AttributeGroupDto>
{
    public class Handler : IRequestHandler<CreateProductAttributeGroup, AttributeGroupDto>
    {
        private readonly IProductsContext _context;

        public Handler(IProductsContext context)
        {
            _context = context;
        }

        public async Task<AttributeGroupDto> Handle(CreateProductAttributeGroup request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .FirstAsync(x => x.Id == request.ProductId);

            var group = new AttributeGroup()
            {
                Id = Guid.NewGuid().ToString(),
                Name = request.Data.Name,
                Description = request.Data.Description
            };

            product.AttributeGroups.Add(group);

            await _context.SaveChangesAsync();

            return new AttributeGroupDto(group.Id, group.Name, group.Description);
        }
    }
}

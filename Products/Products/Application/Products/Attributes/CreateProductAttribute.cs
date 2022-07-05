using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Products.Application.Attributes;
using YourBrand.Products.Domain;
using YourBrand.Products.Domain.Entities;

namespace YourBrand.Products.Application.Products.Attributes;

public record CreateProductAttribute(string ProductId, ApiCreateProductAttribute Data) : IRequest<AttributeDto>
{
    public class Handler : IRequestHandler<CreateProductAttribute, AttributeDto>
    {
        private readonly IProductsContext _context;

        public Handler(IProductsContext context)
        {
            _context = context;
        }

        public async Task<AttributeDto> Handle(CreateProductAttribute request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
            .FirstAsync(attribute => attribute.Id == request.ProductId);

            var group = await _context.AttributeGroups
                .FirstOrDefaultAsync(attribute => attribute.Id == request.Data.GroupId);

            Domain.Entities.Attribute attribute = new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = request.Data.Name,
                Description = request.Data.Description,
                Group = group,
                ForVariant = request.Data.ForVariant
            };

            foreach (var v in request.Data.Values)
            {
                var value = new AttributeValue
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = v.Name
                };

                attribute.Values.Add(value);
            }

            product.Attributes.Add(attribute);

            await _context.SaveChangesAsync();

            return new AttributeDto(attribute.Id, attribute.Name, attribute.Description, attribute.Group == null ? null : new AttributeGroupDto(attribute.Group.Id, attribute.Group.Name, attribute.Group.Description), attribute.ForVariant,
                attribute.Values.Select(attribute => new AttributeValueDto(attribute.Id, attribute.Name, attribute.Seq)));
        
        }
    }
}

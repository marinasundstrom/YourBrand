using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Products.Application.Attributes;
using YourBrand.Products.Domain;
using YourBrand.Products.Domain.Entities;

namespace YourBrand.Products.Application.Products.Attributes;

public record CreateProductAttributeValue(string ProductId, string AttributeId, ApiCreateProductAttributeValue Data) : IRequest<AttributeValueDto>
{
    public class Handler : IRequestHandler<CreateProductAttributeValue, AttributeValueDto>
    {
        private readonly IProductsContext _context;

        public Handler(IProductsContext context)
        {
            _context = context;
        }

        public async Task<AttributeValueDto> Handle(CreateProductAttributeValue request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
            .FirstAsync(x => x.Id == request.ProductId);

            var attribute = await _context.Attributes
                .FirstAsync(x => x.Id == request.AttributeId);

            var value = new AttributeValue
            {
                Name = request.Data.Name
            };

            attribute.Values.Add(value);

            await _context.SaveChangesAsync();

            return new AttributeValueDto(value.Id, value.Name, value.Seq);
        }
    }
}

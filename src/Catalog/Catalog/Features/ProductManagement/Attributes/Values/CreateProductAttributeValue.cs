using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain.Entities;
using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Attributes.Values;

public record CreateProductAttributeValue(string Id, CreateProductAttributeValueData Data) : IRequest<AttributeValueDto>
{
    public class Handler : IRequestHandler<CreateProductAttributeValue, AttributeValueDto>
    {
        private readonly CatalogContext _context;

        public Handler(CatalogContext context)
        {
            _context = context;
        }

        public async Task<AttributeValueDto> Handle(CreateProductAttributeValue request, CancellationToken cancellationToken)
        {
            var attribute = await _context.Attributes
                .FirstAsync(x => x.Id == request.Id);

            var value = new AttributeValue(Guid.NewGuid().ToString())
            {
                Name = request.Data.Name
            };

            attribute.Values.Add(value);

            await _context.SaveChangesAsync(cancellationToken);

            return new AttributeValueDto(value.Id, value.Name, value.Seq);
        }
    }
}
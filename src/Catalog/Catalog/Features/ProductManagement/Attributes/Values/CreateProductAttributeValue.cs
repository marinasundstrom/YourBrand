using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain.Entities;
using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Attributes.Values;

public record CreateProductAttributeValue(string OrganizationId, string Id, CreateProductAttributeValueData Data) : IRequest<AttributeValueDto>
{
    public class Handler(CatalogContext context) : IRequestHandler<CreateProductAttributeValue, AttributeValueDto>
    {
        public async Task<AttributeValueDto> Handle(CreateProductAttributeValue request, CancellationToken cancellationToken)
        {
            var attribute = await context.Attributes
                .InOrganization(request.OrganizationId)
                .FirstAsync(x => x.Id == request.Id);

            var value = new AttributeValue(Guid.NewGuid().ToString())
            {
                OrganizationId = request.OrganizationId,
                Name = request.Data.Name
            };

            attribute.AddValue(value);

            await context.SaveChangesAsync(cancellationToken);

            return new AttributeValueDto(value.Id, value.Name, value.Seq);
        }
    }
}
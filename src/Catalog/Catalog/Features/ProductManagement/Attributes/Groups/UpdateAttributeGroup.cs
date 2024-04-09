using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Attributes.Groups;

public record UpdateAttributeGroup(string Id, UpdateProductAttributeGroupData Data) : IRequest<AttributeGroupDto>
{
    public class Handler(CatalogContext context) : IRequestHandler<UpdateAttributeGroup, AttributeGroupDto>
    {
        public async Task<AttributeGroupDto> Handle(UpdateAttributeGroup request, CancellationToken cancellationToken)
        {
            var attributeGroup = await context.AttributeGroups
                .FirstAsync(x => x.Id == request.Id, cancellationToken);

            attributeGroup.Name = request.Data.Name;
            attributeGroup.Description = request.Data.Description;

            await context.SaveChangesAsync();

            return new AttributeGroupDto(attributeGroup.Id, attributeGroup.Name, attributeGroup.Description);
        }
    }
}
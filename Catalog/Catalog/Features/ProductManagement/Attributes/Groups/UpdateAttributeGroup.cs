using YourBrand.Catalog.Persistence;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Catalog.Features.ProductManagement.Attributes.Groups;

public record UpdateAttributeGroup(string Id, UpdateProductAttributeGroupData Data) : IRequest<AttributeGroupDto>
{
    public class Handler : IRequestHandler<UpdateAttributeGroup, AttributeGroupDto>
    {
        private readonly CatalogContext _context;

        public Handler(CatalogContext context)
        {
            _context = context;
        }

        public async Task<AttributeGroupDto> Handle(UpdateAttributeGroup request, CancellationToken cancellationToken)
        {
            var attributeGroup = await _context.AttributeGroups
                .FirstAsync(x => x.Id == request.Id, cancellationToken);

            attributeGroup.Name = request.Data.Name;
            attributeGroup.Description = request.Data.Description;

            await _context.SaveChangesAsync();

            return new AttributeGroupDto(attributeGroup.Id, attributeGroup.Name, attributeGroup.Description);
        }
    }
}
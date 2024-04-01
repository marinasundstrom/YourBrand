using MediatR;

using YourBrand.Catalog.Domain.Entities;
using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Attributes.Groups;

public record CreateAttributeGroup(CreateProductAttributeGroupData Data) : IRequest<AttributeGroupDto>
{
    public class Handler : IRequestHandler<CreateAttributeGroup, AttributeGroupDto>
    {
        private readonly CatalogContext _context;

        public Handler(CatalogContext context)
        {
            _context = context;
        }

        public async Task<AttributeGroupDto> Handle(CreateAttributeGroup request, CancellationToken cancellationToken)
        {
            var group = new AttributeGroup(Guid.NewGuid().ToString())
            {
                Name = request.Data.Name,
                Description = request.Data.Description
            };

            _context.AttributeGroups.Add(group);

            await _context.SaveChangesAsync(cancellationToken);

            return new AttributeGroupDto(group.Id, group.Name, group.Description);
        }
    }
}
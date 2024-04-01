using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain.Entities;
using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Attributes;

public record CreateAttributeCommand(string Name, string? Description, string? GroupId, IEnumerable<CreateProductAttributeValueData> Values) : IRequest<AttributeDto>
{
    public class CreateAttributeCommandHandler : IRequestHandler<CreateAttributeCommand, AttributeDto>
    {
        private readonly CatalogContext context;

        public CreateAttributeCommandHandler(CatalogContext context)
        {
            this.context = context;
        }

        public async Task<AttributeDto> Handle(CreateAttributeCommand request, CancellationToken cancellationToken)
        {
            var group = await context.AttributeGroups
                .FirstOrDefaultAsync(attribute => attribute.Id == request.GroupId);

            Domain.Entities.Attribute attribute = new(Guid.NewGuid().ToString())
            {
                Name = request.Name,
                Description = request.Description,
                Group = group,
            };

            foreach (var v in request.Values)
            {
                var value = new AttributeValue(Guid.NewGuid().ToString())
                {
                    Name = v.Name
                };

                attribute.Values.Add(value);
            }

            await context.SaveChangesAsync(cancellationToken);

            return attribute.ToDto();
        }
    }
}
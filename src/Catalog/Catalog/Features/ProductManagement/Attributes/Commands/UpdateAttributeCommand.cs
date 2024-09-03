using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain.Entities;
using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Attributes;

public record UpdateAttributeCommand(string OrganizationId, string Id, string Name, string? Description, string? GroupId, IEnumerable<UpdateProductAttributeValueData> Values) : IRequest
{
    public class UpdateAttributeCommandHandler(CatalogContext context) : IRequestHandler<UpdateAttributeCommand>
    {
        public async Task Handle(UpdateAttributeCommand request, CancellationToken cancellationToken)
        {
            var attribute = await context.Attributes
                .InOrganization(request.OrganizationId)
                .Include(x => x.Values)
                .Include(x => x.Group)
                .FirstAsync(x => x.Id == request.Id);

            var group = await context.AttributeGroups
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.GroupId);

            attribute.Name = request.Name;
            attribute.Description = request.Description;
            attribute.Group = group;

            foreach (var v in request.Values)
            {
                if (v.Id == null)
                {
                    var value = new AttributeValue(Guid.NewGuid().ToString())
                    {
                        Name = v.Name
                    };

                    attribute.AddValue(value);
                    context.AttributeValues.Add(value);
                }
                else
                {
                    var value = attribute.Values.First(x => x.Id == v.Id);

                    value.Name = v.Name;
                }
            }

            foreach (var v in attribute.Values.ToList())
            {
                if (context.Entry(v).State == EntityState.Added)
                    continue;

                var value = request.Values.FirstOrDefault(x => x.Id == v.Id);

                if (value is null)
                {
                    attribute.RemoveValue(v);
                }
            }

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Application.Attributes;
using YourBrand.Catalog.Domain;
using YourBrand.Catalog.Domain.Entities;

namespace YourBrand.Catalog.Application.Products.Attributes;

public record UpdateProductAttribute(string ProductId, string AttributeId, ApiUpdateProductAttribute Data) : IRequest<AttributeDto>
{
    public class Handler : IRequestHandler<UpdateProductAttribute, AttributeDto>
    {
        private readonly ICatalogContext _context;

        public Handler(ICatalogContext context)
        {
            _context = context;
        }

        public async Task<AttributeDto> Handle(UpdateProductAttribute request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
            .AsNoTracking()
            .FirstAsync(x => x.Id == request.ProductId);

            var attribute = await _context.Attributes
                .Include(x => x.Values)
                .Include(x => x.Group)
                .FirstAsync(x => x.Id == request.AttributeId);

            var group = await _context.AttributeGroups
                .FirstOrDefaultAsync(x => x.Id == request.Data.GroupId);

            attribute.Name = request.Data.Name;
            attribute.Description = request.Data.Description;
            attribute.Group = group;
            attribute.ForVariant = request.Data.ForVariant;

            foreach (var v in request.Data.Values)
            {
                if (v.Id == null)
                {
                    var value = new AttributeValue
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = v.Name
                    };

                    attribute.Values.Add(value);
                    _context.AttributeValues.Add(value);
                }
                else
                {
                    var value = attribute.Values.First(x => x.Id == v.Id);

                    value.Name = v.Name;
                }
            }

            foreach (var v in attribute.Values.ToList())
            {
                if (_context.Entry(v).State == EntityState.Added)
                    continue;

                var value = request.Data.Values.FirstOrDefault(x => x.Id == v.Id);

                if (value is null)
                {
                    attribute.Values.Remove(v);
                }
            }

            await _context.SaveChangesAsync();

            return new AttributeDto(attribute.Id, attribute.Name, attribute.Description, attribute.Group == null ? null : new AttributeGroupDto(attribute.Group.Id, attribute.Group.Name, attribute.Group.Description), attribute.ForVariant,         
                    attribute.Values.Select(attribute => new AttributeValueDto(attribute.Id, attribute.Name, attribute.Seq)));
        }
    }
}

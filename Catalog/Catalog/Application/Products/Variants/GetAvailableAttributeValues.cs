using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Application.Attributes;
using YourBrand.Catalog.Domain;
using YourBrand.Catalog.Domain.Entities;

namespace YourBrand.Catalog.Application.Products.Variants;

public record GetAvailableAttributeValues(string ProductId, string AttributeId, IDictionary<string, string?> SelectedAttributeValues) : IRequest<IEnumerable<AttributeValueDto>>
{
    public class Handler : IRequestHandler<GetAvailableAttributeValues, IEnumerable<AttributeValueDto>>
    {
        private readonly ICatalogContext _context;

        public Handler(ICatalogContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AttributeValueDto>> Handle(GetAvailableAttributeValues request, CancellationToken cancellationToken)
        {
            IEnumerable<ProductVariant> variants = await _context.ProductVariants
                .AsSplitQuery()
                .AsNoTracking()
                .Include(pv => pv.Product)
                .Include(pv => pv.Values)
                .ThenInclude(pv => pv.Attribute)
                .Include(pv => pv.Values)
                .ThenInclude(pv => pv.Value)
                .Where(pv => pv.Product.Id == request.ProductId)
                .ToArrayAsync();

            foreach (var selectedAttribute in request.SelectedAttributeValues)
            {
                if (selectedAttribute.Value is null)
                    continue;

                variants = variants.Where(x => x.Values.Any(vv => vv.Attribute.Id == selectedAttribute.Key && vv.Value.Id == selectedAttribute.Value));
            }

            var values = variants
                .SelectMany(x => x.Values)
                .DistinctBy(x => x.Attribute)
                .Where(x => x.Attribute.Id == request.AttributeId)
                .Select(x => x.Value);

            return values.Select(x => new AttributeValueDto(x.Id, x.Name, x.Seq));
        }
    }
}

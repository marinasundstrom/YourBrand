using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Products.Domain;

namespace YourBrand.Products.Application.Attributes;

public record GetAttributeValues(string AttributeId) : IRequest<IEnumerable<AttributeValueDto>>
{
    public class Handler : IRequestHandler<GetAttributeValues, IEnumerable<AttributeValueDto>>
    {
        private readonly IProductsContext _context;

        public Handler(IProductsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AttributeValueDto>> Handle(GetAttributeValues request, CancellationToken cancellationToken)
        {
            var options = await _context.AttributeValues
                .AsSplitQuery()
                .AsNoTracking()
                .Include(pv => pv.Attribute)
                .ThenInclude(pv => pv.Group)
                .Where(p => p.Attribute.Id == request.AttributeId)
                .ToArrayAsync();

            return options.Select(x => new AttributeValueDto(x.Id, x.Name, x.Seq));  
        }
    }
}

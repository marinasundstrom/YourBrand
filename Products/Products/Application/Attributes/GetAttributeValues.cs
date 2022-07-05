using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Products.Domain;

namespace YourBrand.Products.Application.Attributes;

public record GetAttributeValues(string AttributeId) : IRequest<IEnumerable<ApiAttributeValue>>
{
    public class Handler : IRequestHandler<GetAttributeValues, IEnumerable<ApiAttributeValue>>
    {
        private readonly IProductsContext _context;

        public Handler(IProductsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ApiAttributeValue>> Handle(GetAttributeValues request, CancellationToken cancellationToken)
        {
            var options = await _context.AttributeValues
                .AsSplitQuery()
                .AsNoTracking()
                .Include(pv => pv.Attribute)
                .ThenInclude(pv => pv.Group)
                .Where(p => p.Attribute.Id == request.AttributeId)
                .ToArrayAsync();

            return options.Select(x => new ApiAttributeValue(x.Id, x.Name, x.Seq));  
        }
    }
}

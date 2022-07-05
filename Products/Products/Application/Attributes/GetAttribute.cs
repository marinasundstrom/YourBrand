using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Products.Domain;

namespace YourBrand.Products.Application.Attributes;

public record GetAttribute(string AttributeId) : IRequest<ApiAttribute>
{
    public class Handler : IRequestHandler<GetAttribute, ApiAttribute>
    {
        private readonly IProductsContext _context;

        public Handler(IProductsContext context)
        {
            _context = context;
        }

        public async Task<ApiAttribute> Handle(GetAttribute request, CancellationToken cancellationToken)
        {
            var attribute = await _context.Attributes
                .AsSplitQuery()
                .AsNoTracking()
                .Include(pv => pv.Group)
                .Include(pv => pv.Values)
                .FirstAsync(o => o.Id == request.AttributeId);

            return new ApiAttribute(attribute.Id, attribute.Name, attribute.Description, attribute.Group == null ? null : new ApiAttributeGroup(attribute.Group.Id, attribute.Group.Name, attribute.Group.Description),
                attribute.Values.Select(x => new ApiAttributeValue(x.Id, x.Name, x.Seq)));
        }
    }
}

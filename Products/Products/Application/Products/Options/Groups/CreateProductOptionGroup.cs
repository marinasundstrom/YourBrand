using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Products.Domain;
using YourBrand.Products.Domain.Entities;

namespace YourBrand.Products.Application.Products.Options.Groups;

public record CreateProductOptionGroup(string ProductId, ApiCreateProductOptionGroup Data) : IRequest<ApiOptionGroup>
{
    public class Handler : IRequestHandler<CreateProductOptionGroup, ApiOptionGroup>
    {
        private readonly IProductsContext _context;

        public Handler(IProductsContext context)
        {
            _context = context;
        }

        public async Task<ApiOptionGroup> Handle(CreateProductOptionGroup request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .FirstAsync(x => x.Id == request.ProductId);

            var group = new OptionGroup()
            {
                Id = Guid.NewGuid().ToString(),
                Name = request.Data.Name,
                Description = request.Data.Description,
                Min = request.Data.Min,
                Max = request.Data.Max
            };

            product.OptionGroups.Add(group);

            await _context.SaveChangesAsync();

            return new ApiOptionGroup(group.Id, group.Name, group.Description, group.Seq, group.Min, group.Max);
        }
    }
}

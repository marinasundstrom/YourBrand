using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Products.Domain;
using YourBrand.Products.Domain.Entities;

namespace YourBrand.Products.Application.Products.Groups;

public record CreateProductGroup(string ProductId, ApiCreateProductGroup Data) : IRequest<ApiProductGroup>
{
    public class Handler : IRequestHandler<CreateProductGroup, ApiProductGroup>
    {
        private readonly IProductsContext _context;

        public Handler(IProductsContext context)
        {
            _context = context;
        }

        public async Task<ApiProductGroup> Handle(CreateProductGroup request, CancellationToken cancellationToken)
        {
            var parentGroup = await _context.ProductGroups
            .FirstOrDefaultAsync(x => x.Id == request.Data.ParentGroupId);

            var productGroup = new ProductGroup()
            {
                Id = Guid.NewGuid().ToString(),
                Name = request.Data.Name,
                Description = request.Data.Description,
                Parent = parentGroup
            };

            _context.ProductGroups.Add(productGroup);

            await _context.SaveChangesAsync();

            return new ApiProductGroup(productGroup.Id, productGroup.Name, productGroup.Description, productGroup?.Parent?.Id);

        }
    }
}

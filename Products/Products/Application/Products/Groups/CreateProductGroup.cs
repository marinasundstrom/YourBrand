using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Products.Domain;
using YourBrand.Products.Domain.Entities;

namespace YourBrand.Products.Application.Products.Groups;

public record CreateProductGroup(string ProductId, ApiCreateProductGroup Data) : IRequest<ProductGroupDto>
{
    public class Handler : IRequestHandler<CreateProductGroup, ProductGroupDto>
    {
        private readonly IProductsContext _context;

        public Handler(IProductsContext context)
        {
            _context = context;
        }

        public async Task<ProductGroupDto> Handle(CreateProductGroup request, CancellationToken cancellationToken)
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

            return new ProductGroupDto(productGroup.Id, productGroup.Name, productGroup.Description, productGroup?.Parent?.Id);

        }
    }
}

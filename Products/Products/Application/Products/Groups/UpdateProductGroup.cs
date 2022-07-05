using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Products.Domain;
using YourBrand.Products.Domain.Entities;

namespace YourBrand.Products.Application.Products.Groups;

public record UpdateProductGroup(string ProductGroupId, ApiUpdateProductGroup Data) : IRequest<ProductGroupDto>
{
    public class Handler : IRequestHandler<UpdateProductGroup, ProductGroupDto>
    {
        private readonly IProductsContext _context;

        public Handler(IProductsContext context)
        {
            _context = context;
        }

        public async Task<ProductGroupDto> Handle(UpdateProductGroup request, CancellationToken cancellationToken)
        {
            var productGroup = await _context.ProductGroups
                    .FirstAsync(x => x.Id == request.ProductGroupId);

            var parentGroup = await _context.ProductGroups
                .FirstOrDefaultAsync(x => x.Id == request.Data.ParentGroupId);

            productGroup.Name = request.Data.Name;
            productGroup.Description = request.Data.Description;
            productGroup.Parent = parentGroup;

            await _context.SaveChangesAsync();

            return new ProductGroupDto(productGroup.Id, productGroup.Name, productGroup.Description, productGroup?.Parent?.Id);
        }
    }
}

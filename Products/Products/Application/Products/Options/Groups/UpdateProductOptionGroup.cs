using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Products.Domain;
using YourBrand.Products.Domain.Entities;

namespace YourBrand.Products.Application.Products.Options.Groups;

public record UpdateProductOptionGroup(string ProductId, string OptionGroupId, ApiUpdateProductOptionGroup Data) : IRequest<ApiOptionGroup>
{
    public class Handler : IRequestHandler<UpdateProductOptionGroup, ApiOptionGroup>
    {
        private readonly IProductsContext _context;

        public Handler(IProductsContext context)
        {
            _context = context;
        }

        public async Task<ApiOptionGroup> Handle(UpdateProductOptionGroup request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
            .Include(x => x.OptionGroups)
            .FirstAsync(x => x.Id == request.ProductId);

            var optionGroup = product.OptionGroups
                .First(x => x.Id == request.OptionGroupId);

            optionGroup.Name = request.Data.Name;
            optionGroup.Description = request.Data.Description;
            optionGroup.Min = request.Data.Min;
            optionGroup.Max = request.Data.Max;

            await _context.SaveChangesAsync();

            return new ApiOptionGroup(optionGroup.Id, optionGroup.Name, optionGroup.Description, optionGroup.Seq, optionGroup.Min, optionGroup.Max);
        }
    }
}

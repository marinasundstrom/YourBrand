using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain.Entities;
using YourBrand.Catalog.Features.ProductManagement.Options;
using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Products.Options.Groups;

public record CreateProductOptionGroup(long ProductId, CreateProductOptionGroupData Data) : IRequest<OptionGroupDto>
{
    public class Handler(CatalogContext context) : IRequestHandler<CreateProductOptionGroup, OptionGroupDto>
    {
        public async Task<OptionGroupDto> Handle(CreateProductOptionGroup request, CancellationToken cancellationToken)
        {
            var product = await context.Products
                .FirstAsync(x => x.Id == request.ProductId);

            var group = new OptionGroup(Guid.NewGuid().ToString())
            {
                Name = request.Data.Name,
                Description = request.Data.Description,
                Min = request.Data.Min,
                Max = request.Data.Max
            };

            product.AddOptionGroup(group);

            await context.SaveChangesAsync();

            return new OptionGroupDto(group.Id, group.Name, group.Description, group.Seq, group.Min, group.Max);
        }
    }
}
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Application.Options;
using YourBrand.Catalog.Domain;
using YourBrand.Catalog.Domain.Entities;

namespace YourBrand.Catalog.Application.Products.Options.Groups;

public record CreateProductOptionGroup(string ProductId, ApiCreateProductOptionGroup Data) : IRequest<OptionGroupDto>
{
    public class Handler : IRequestHandler<CreateProductOptionGroup, OptionGroupDto>
    {
        private readonly ICatalogContext _context;

        public Handler(ICatalogContext context)
        {
            _context = context;
        }

        public async Task<OptionGroupDto> Handle(CreateProductOptionGroup request, CancellationToken cancellationToken)
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

            return new OptionGroupDto(group.Id, group.Name, group.Description, group.Seq, group.Min, group.Max);
        }
    }
}

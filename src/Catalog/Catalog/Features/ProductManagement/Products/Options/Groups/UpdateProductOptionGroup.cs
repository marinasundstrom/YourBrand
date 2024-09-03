using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Features.ProductManagement.Options;
using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Products.Options.Groups;

public record UpdateProductOptionGroup(string OrganizationId, long ProductId, string OptionGroupId, UpdateProductOptionGroupData Data) : IRequest<OptionGroupDto>
{
    public class Handler(CatalogContext context) : IRequestHandler<UpdateProductOptionGroup, OptionGroupDto>
    {
        public async Task<OptionGroupDto> Handle(UpdateProductOptionGroup request, CancellationToken cancellationToken)
        {
            var product = await context.Products
            .InOrganization(request.OrganizationId)
            .Include(x => x.OptionGroups)
            .FirstAsync(x => x.Id == request.ProductId);

            var optionGroup = product.OptionGroups
                .First(x => x.Id == request.OptionGroupId);

            optionGroup.Name = request.Data.Name;
            optionGroup.Description = request.Data.Description;
            optionGroup.Min = request.Data.Min;
            optionGroup.Max = request.Data.Max;

            await context.SaveChangesAsync();

            return new OptionGroupDto(optionGroup.Id, optionGroup.Name, optionGroup.Description, optionGroup.Seq, optionGroup.Min, optionGroup.Max);
        }
    }
}
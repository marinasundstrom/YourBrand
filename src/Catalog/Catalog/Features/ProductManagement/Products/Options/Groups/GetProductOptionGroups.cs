using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Features.ProductManagement.Options;
using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Products.Options.Groups;

public record GetProductOptionGroups(string OrganizationId, long ProductId) : IRequest<IEnumerable<OptionGroupDto>>
{
    public class Handler(CatalogContext context) : IRequestHandler<GetProductOptionGroups, IEnumerable<OptionGroupDto>>
    {
        public async Task<IEnumerable<OptionGroupDto>> Handle(GetProductOptionGroups request, CancellationToken cancellationToken)
        {
            var groups = await context.OptionGroups
            .InOrganization(request.OrganizationId)
            .AsTracking()
            .Include(x => x.Product)
            .Where(x => x.Product!.Id == request.ProductId)
            .ToListAsync();

            return groups.Select(group => new OptionGroupDto(group.Id, group.Name, group.Description, group.Seq, group.Min, group.Max));
        }
    }
}
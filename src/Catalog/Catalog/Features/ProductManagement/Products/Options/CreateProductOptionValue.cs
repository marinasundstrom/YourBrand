using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain.Entities;
using YourBrand.Catalog.Features.ProductManagement.Options;
using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Products.Options;

public record CreateProductOptionValue(string OrganizationId, long ProductId, string OptionId, CreateProductOptionValueData Data) : IRequest<OptionValueDto>
{
    public class Handler(CatalogContext context) : IRequestHandler<CreateProductOptionValue, OptionValueDto>
    {
        public async Task<OptionValueDto> Handle(CreateProductOptionValue request, CancellationToken cancellationToken)
        {
            var product = await context.Products
            .InOrganization(request.OrganizationId)
            .FirstAsync(x => x.Id == request.ProductId);

            var option = await context.Options
                .InOrganization(request.OrganizationId)
                .FirstAsync(x => x.Id == request.OptionId);

            var value = new OptionValue(request.Data.Name)
            {
                OrganizationId = request.OrganizationId,
                SKU = request.Data.SKU,
                Price = request.Data.Price
            };

            (option as ChoiceOption)!.AddValue(value);

            await context.SaveChangesAsync();

            return new OptionValueDto(value.Id, value.Name, value.SKU, value.Price, value.Seq);
        }
    }
}
using MediatR;

using YourBrand.Catalog.Features.ProductManagement.Attributes;

namespace YourBrand.Catalog.Features.ProductManagement.Products.Variants;

public record GetAvailableAttributeValues(string OrganizationId, string ProductIdOrHandle, string AttributeId, IDictionary<string, string?> SelectedAttributeValues) : IRequest<IEnumerable<AttributeValueDto>>
{
    public class Handler(ProductVariantsService productsService) : IRequestHandler<GetAvailableAttributeValues, IEnumerable<AttributeValueDto>>
    {
        public async Task<IEnumerable<AttributeValueDto>> Handle(GetAvailableAttributeValues request, CancellationToken cancellationToken)
        {
            var values = await productsService.GetAvailableAttributeValues(request.OrganizationId, request.ProductIdOrHandle, request.AttributeId, request.SelectedAttributeValues, cancellationToken);
            return values.Select(x => new AttributeValueDto(x!.Id, x.Name, x.Seq));
        }
    }
}
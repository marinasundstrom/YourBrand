using MediatR;

using YourBrand.Catalog.Features.ProductManagement.Attributes;

namespace YourBrand.Catalog.Features.ProductManagement.Products.Variants;

public record GetAvailableAttributeValues(string ProductIdOrHandle, string AttributeId, IDictionary<string, string?> SelectedAttributeValues) : IRequest<IEnumerable<AttributeValueDto>>
{
    public class Handler : IRequestHandler<GetAvailableAttributeValues, IEnumerable<AttributeValueDto>>
    {
        private readonly ProductVariantsService _productsService;

        public Handler(ProductVariantsService productsService)
        {
            _productsService = productsService;
        }

        public async Task<IEnumerable<AttributeValueDto>> Handle(GetAvailableAttributeValues request, CancellationToken cancellationToken)
        {
            var values = await _productsService.GetAvailableAttributeValues(request.ProductIdOrHandle, request.AttributeId, request.SelectedAttributeValues, cancellationToken);
            return values.Select(x => new AttributeValueDto(x!.Id, x.Name, x.Seq));
        }
    }
}
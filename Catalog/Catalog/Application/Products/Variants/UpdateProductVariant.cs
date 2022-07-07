using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain;
using YourBrand.Catalog.Domain.Entities;

namespace YourBrand.Catalog.Application.Products.Variants;

public record UpdateProductVariant(string ProductId, string ProductVariantId, ApiUpdateProductVariant Data) : IRequest<ProductVariantDto>
{
    public class Handler : IRequestHandler<UpdateProductVariant, ProductVariantDto>
    {
        private readonly ICatalogContext _context;
        private ProductVariantsService _productVariantsService;

        public Handler(ICatalogContext context, ProductVariantsService productVariantsService)
        {
            _context = context;
            _productVariantsService = productVariantsService;
        }
        
        public async Task<ProductVariantDto> Handle(UpdateProductVariant request, CancellationToken cancellationToken)
        {
            var match = await _productVariantsService.FindVariantCore(request.ProductId, request.ProductVariantId, request.Data.Options.ToDictionary(x => x.OptionId, x => x.ValueId));

            if (match is not null)
            {
                throw new VariantAlreadyExistsException("Variant with the same options already exists.");
            }

            var product = await _context.Products
                .AsSplitQuery()
                .Include(pv => pv.Variants)
                    .ThenInclude(o => o.Values)
                    .ThenInclude(o => o.Attribute)
                .Include(pv => pv.Variants)
                    .ThenInclude(o => o.Values)
                    .ThenInclude(o => o.Value)
                .Include(pv => pv.Attributes)
                    .ThenInclude(o => o.Values)
                .FirstAsync(x => x.Id == request.ProductId);

            var variant = product.Variants.First(x => x.Id == request.ProductVariantId);

            variant.Name = request.Data.Name;
            variant.Description = request.Data.Description;
            variant.SKU = request.Data.SKU;
            variant.Price = request.Data.Price;

            foreach (var v in request.Data.Options)
            {
                if (v.Id == null)
                {
                    var option = product.Attributes.First(x => x.Id == v.OptionId);

                    var value2 = option.Values.First(x => x.Id == v.ValueId);

                    variant.Values.Add(new VariantValue()
                    {
                        Attribute = option,
                        Value = value2
                    });
                }
                else
                {
                    var option = product.Attributes.First(x => x.Id == v.OptionId);

                    var value2 = option.Values.First(x => x.Id == v.ValueId);

                    var value = variant.Values.First(x => x.Id == v.Id);

                    value.Attribute = option;
                    value.Value = value2;
                }
            }

            foreach (var v in variant.Values.ToList())
            {
                if (_context.Entry(v).State == EntityState.Added)
                    continue;

                var value = request.Data.Options.FirstOrDefault(x => x.Id == v.Id);

                if (value is null)
                {
                    variant.Values.Remove(v);
                }
            }

            await _context.SaveChangesAsync();

            return new ProductVariantDto(variant.Id, variant.Name, variant.Description, variant.SKU, GetImageUrl(variant.Image), variant.Price,
                variant.Values.Select(x => new ProductVariantDtoOption(x.Attribute.Id, x.Attribute.Name, x.Value.Name)));
        }

        private static string? GetImageUrl(string? name)
        {
            return name is null ? null : $"http://127.0.0.1:10000/devstoreaccount1/images/{name}";
        }
    }
}

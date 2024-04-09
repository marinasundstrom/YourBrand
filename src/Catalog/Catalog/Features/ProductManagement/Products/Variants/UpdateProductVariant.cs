using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain.Entities;
using YourBrand.Catalog.Persistence;
namespace YourBrand.Catalog.Features.ProductManagement.Products.Variants;

public record UpdateProductVariant(long ProductId, long ProductVariantId, UpdateProductVariantData Data) : IRequest<ProductDto>
{
    public class Handler(CatalogContext context, ProductVariantsService productVariantsService) : IRequestHandler<UpdateProductVariant, ProductDto>
    {
        public async Task<ProductDto> Handle(UpdateProductVariant request, CancellationToken cancellationToken)
        {
            var match = (await productVariantsService.FindVariants(request.ProductId.ToString(), request.ProductVariantId.ToString(), request.Data.Attributes.ToDictionary(x => x.AttributeId, x => x.ValueId)!, cancellationToken))
                .SingleOrDefault();

            if (match is not null)
            {
                throw new VariantAlreadyExistsException("Variant with the same options already exists.");
            }

            var product = await context.Products
                .AsSplitQuery()
                .Include(pv => pv.Parent)
                    .ThenInclude(pv => pv!.Category)
                .Include(pv => pv.Variants)
                    .ThenInclude(o => o.ProductAttributes)
                    .ThenInclude(o => o.Attribute)
                .Include(pv => pv.Variants)
                    .ThenInclude(o => o.ProductAttributes)
                    .ThenInclude(o => o.Value)
                .FirstAsync(x => x.Id == request.ProductId);

            var variant = product.Variants.First(x => x.Id == request.ProductVariantId);

            variant.Name = request.Data.Name;
            variant.Description = request.Data.Description;
            //variant.Price = request.Data.Price;
            //variant.RegularPrice = request.Data.RegularPrice;

            foreach (var v in request.Data.Attributes)
            {
                if (v.Id == null)
                {
                    var attribute = context.Attributes.First(x => x.Id == v.AttributeId);

                    var value2 = attribute.Values.First(x => x.Id == v.ValueId);

                    variant.AddProductAttribute(new ProductAttribute()
                    {
                        Attribute = attribute,
                        Value = value2
                    });
                }
                else
                {
                    var option = context.Attributes.First(x => x.Id == v.AttributeId);

                    var value2 = option.Values.First(x => x.Id == v.ValueId);

                    var value = variant.ProductAttributes.First(x => x.Id == v.Id);

                    value.Attribute = option;
                    value.Value = value2;
                }
            }

            foreach (var v in variant.ProductAttributes.ToList())
            {
                if (context.Entry(v).State == EntityState.Added)
                    continue;

                var value = request.Data.Attributes.FirstOrDefault(x => x.Id == v.Id);

                if (value is null)
                {
                    variant.RemoveProductAttribute(v);
                }
            }

            await context.SaveChangesAsync();

            return variant.ToDto();
        }

        private static string? GetImageUrl(string? name)
        {
            return name is null ? null : $"http://127.0.0.1:10000/devstoreaccount1/images/{name}";
        }
    }
}
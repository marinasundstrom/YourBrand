using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain.Entities;
using YourBrand.Catalog.Features.ProductManagement.Products.Images;
using YourBrand.Catalog.Persistence;
namespace YourBrand.Catalog.Features.ProductManagement.Products.Variants;

public record CreateProductVariant(long ProductId, CreateProductVariantData Data) : IRequest<ProductDto>
{
    public class Handler : IRequestHandler<CreateProductVariant, ProductDto>
    {
        private readonly CatalogContext _context;
        private readonly ProductVariantsService _productVariantsService;
        private readonly IProductImageUploader _productImageUploader;
        private readonly IConfiguration _configuration;

        public Handler(CatalogContext context, ProductVariantsService productVariantsService, IProductImageUploader productImageUploader, IConfiguration configuration)
        {
            _context = context;
            _productVariantsService = productVariantsService;
            _productImageUploader = productImageUploader;
            _configuration = configuration;
        }

        public async Task<ProductDto> Handle(CreateProductVariant request, CancellationToken cancellationToken)
        {
            Product? match = (await _productVariantsService.FindVariants(request.ProductId.ToString(), null, request.Data.Attributes.ToDictionary(x => x.AttributeId, x => x.ValueId)!, cancellationToken))
                .SingleOrDefault();

            if (match is not null)
            {
                throw new VariantAlreadyExistsException("Variant with the same options already exists.");
            }

            var handleInUse = await _context.Products.AnyAsync(product => product.Handle == request.Data.Handle, cancellationToken);

            if (handleInUse)
            {
                return Result.Failure<ProductDto>(Errors.HandleAlreadyTaken);
            }

            var product = await _context.Products
                .AsSplitQuery()
                .IncludeAll()
                // .Include(pv => pv.Parent)
                //     .ThenInclude(pv => pv!.Category)
                // .Include(pv => pv.Variants)
                //     .ThenInclude(o => o.ProductAttributes)
                //     .ThenInclude(o => o.Attribute)
                // .Include(pv => pv.Variants)
                //     .ThenInclude(o => o.ProductAttributes)
                //     .ThenInclude(o => o.Value)
                .FirstAsync(x => x.Id == request.ProductId);

            var connectionString = _context.Database.GetConnectionString()!;

            string cdnBaseUrl = (connectionString.Contains("localhost") || connectionString.Contains("mssql"))
                ? _configuration["CdnBaseUrl"]!
                : "https://yourbrandstorage.blob.core.windows.net";

            var variant = new Domain.Entities.Product()
            {
                Name = request.Data.Name,
                Handle = request.Data.Handle,
                Description = request.Data.Description ?? string.Empty
            };

            product.AddVariant(variant);

            variant.SetPrice(request.Data.Price);

            var image = new ProductImage("Placeholder", string.Empty, await _productImageUploader.GetPlaceholderImageUrl());
            variant.AddImage(image);

            foreach (var value in request.Data.Attributes)
            {
                var attribute = _context.Attributes
                    .Include(x => x.Values)
                    .First(x => x.Id == value.AttributeId);

                var value2 = attribute.Values.First(x => x.Id == value.ValueId);

                variant.AddProductAttribute(new ProductAttribute()
                {
                    Attribute = attribute,
                    Value = value2
                });
            }

            product.AddVariant(variant);

            await _context.SaveChangesAsync();

            variant.Image = image;

            await _context.SaveChangesAsync();

            //var p = await _context.Products
            //    .IncludeAll()
            //    .FirstAsync(x => x.Id == variant.Id, cancellationToken);

            return variant.ToDto();
        }

        private static string? GetImageUrl(string? name)
        {
            return name is null ? null : $"http://127.0.0.1:10000/devstoreaccount1/images/{name}";
        }
    }
}
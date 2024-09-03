using System.Globalization;
using System.Reflection;

using CsvHelper;
using CsvHelper.Configuration;

using MediatR;

using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain.Entities;
using YourBrand.Catalog.Features.ProductManagement.Products.Images;
using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Import;

public record ProductImportResult(IEnumerable<string> Diagnostics);

public sealed record ImportProducts(string OrganizationId, Stream Stream) : IRequest<Result<ProductImportResult>>
{
    public sealed class Handler(CatalogContext context, IProductImageUploader productImageUploader) : IRequestHandler<ImportProducts, Result<ProductImportResult>>
    {
        public async Task<Result<ProductImportResult>> Handle(ImportProducts request, CancellationToken cancellationToken)
        {
            var name = DateTime.UtcNow.Ticks.ToString();

            await UploadAndExtractFiles(request.Stream, name);

            string filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"uploads/{name}/products.csv");

            using var fileStream = File.OpenRead(filePath);

            List<string> diagnostics = new();

            var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = ","
            };

            using (var reader = new StreamReader(fileStream))
            using (var csv = new CsvReader(reader, configuration))
            {
                var records = csv.GetRecords<ProductRecord>();

                foreach (var record in records)
                {
                    Store store = await GetStore(request.OrganizationId, record.StoreIdOrHandle, cancellationToken);

                    string productHandle = GetHandle(record);

                    var productExists = context.Products
                        .Where(x => x.Store == store)
                        .Any(x => x.Sku == record.Sku || x.Handle == record.Handle);

                    if (productExists)
                    {
                        diagnostics.Add($"Product with SKU \"{record.Sku}\" already exists. Skipping it.");
                        continue;
                    }

                    Brand? brand = string.IsNullOrEmpty(record.Brand) ? null : await GetBrand(request.OrganizationId, record.Brand, cancellationToken);

                    ProductCategory category = await GetCategory(request.OrganizationId, store, record.CategoryId.GetValueOrDefault(), cancellationToken);

                    var parentProduct = string.IsNullOrEmpty(record.ParentSku) ? null : await GetProduct(request.OrganizationId, store, record.ParentSku, cancellationToken);

                    var product = new Product(record.Name, productHandle)
                    {
                        Sku = record.Sku,
                        Description = record.Description ?? string.Empty,
                        //Image = record.Image,
                        Parent = parentProduct,
                        Store = store,
                        ListingState = record.Listed.GetValueOrDefault() ? Domain.Enums.ProductListingState.Listed : Domain.Enums.ProductListingState.Unlisted
                    };

                    if (record.RegularPrice is not null)
                    {
                        product.SetPrice(record.RegularPrice.GetValueOrDefault());
                    }

                    product.SetDiscountPrice(record.Price);

                    products.Add(record.Sku, product);

                    category.AddProduct(product);
                }
            }

            context.Products.AddRange(products.Select(x => x.Value));

            await context.SaveChangesAsync(cancellationToken);

            string ArchiveDirPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"uploads/{name}");

            foreach (var product in products.Select(x => x.Value))
            {
                if (product.Image is null)
                {
                    continue;
                }

                var image = product.Image is null ? null : Path.Combine(ArchiveDirPath, product.Image.Url!);

                var fileName = product.Image.Url;

                await productImageUploader.TryDeleteProductImage(product.Id, fileName);

                Stream? stream = null;

                try
                {
                    stream = File.OpenRead(image!);
                }
                catch (FileNotFoundException)
                {
                    diagnostics.Add($"Image \"{image}\" not found for SKU \"{product.Sku}\".");

                    continue;
                }

                try
                {
                    string? path = null;

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        var mimeType = GetMimeTypeForFileExtension(fileName);
                        path = await productImageUploader.UploadProductImage(product.Id, fileName, stream!, mimeType);
                    }
                    else
                    {
                        path = await productImageUploader.GetPlaceholderImageUrl();
                    }

                    var image2 = new ProductImage("Image", string.Empty, path);
                    image2.OrganizationId = request.OrganizationId;
                    product.AddImage(image2);
                    product.Image = image2;

                    File.Delete(image!);
                }
                catch (Exception exc)
                {
                    diagnostics.Add($"{exc.Message}");
                }
            }

            await context.SaveChangesAsync(cancellationToken);

            Directory.Delete(ArchiveDirPath, true);

            return Result.Success(new ProductImportResult(diagnostics));
        }

        private static string GetHandle(ProductRecord record)
        {
            return record.Handle ?? record.Name
                .ToLower()
                .Replace("-", string.Empty);
        }

        readonly Dictionary<long, ProductCategory> categories = new Dictionary<long, ProductCategory>();

        private async Task<ProductCategory> GetCategory(string organizationId, Store store, long categoryId, CancellationToken cancellationToken)
        {
            if (!categories.TryGetValue(categoryId, out var category))
            {
                category = await context
                    .ProductCategories
                    .InOrganization(organizationId)
                    .Where(x => x.Store == store)
                    .Include(x => x.Parent)
                    .FirstAsync(x => x.Id == categoryId, cancellationToken);

                categories.Add(categoryId, category);
            }
            return category;
        }

        readonly Dictionary<string, Brand> brands = new Dictionary<string, Brand>();

        private async Task<Brand> GetBrand(string organizationId, string handle, CancellationToken cancellationToken)
        {
            if (!brands.TryGetValue(handle, out var brand))
            {
                brand = await context.Brands
                        .InOrganization(organizationId)
                        .FirstAsync(x => x.Handle == handle, cancellationToken);
                brands.Add(handle, brand);
            }
            return brand;
        }

        readonly Dictionary<string, Store> stores = new Dictionary<string, Store>();

        private async Task<Store> GetStore(string organizationId, string handle, CancellationToken cancellationToken)
        {
            if (!stores.TryGetValue(handle, out var store))
            {
                store = await context.Stores
                        .InOrganization(organizationId)
                        .FirstAsync(x => x.Handle == handle, cancellationToken);
                stores.Add(handle, store);
            }
            return store;
        }

        readonly Dictionary<string, Product> products = new Dictionary<string, Product>();

        private async Task<Product> GetProduct(string organizationId, Store store, string sku, CancellationToken cancellationToken)
        {
            if (!products.TryGetValue(sku, out var product))
            {
                product = await context.Products
                    .InOrganization(organizationId)
                    .Where(x => x.Sku == sku)
                    .FirstAsync(x => x.Sku == sku, cancellationToken);

                products.Add(sku, product);
            }
            return product;
        }

        async Task UploadAndExtractFiles(Stream stream, string name)
        {
            string UploadDirPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"uploads");

            try
            {
                Directory.CreateDirectory(UploadDirPath);
            }
            catch (IOException) { }

            string ArchiveFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"uploads/{name}.zip");

            using (var file = File.Open(ArchiveFilePath, FileMode.OpenOrCreate))
            {
                await stream.CopyToAsync(file);
                file.Seek(0, SeekOrigin.Begin);
            }

            string ArchiveDirPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"uploads/{name}");

            try
            {
                Directory.CreateDirectory(ArchiveDirPath);
            }
            catch (IOException) { }

            System.IO.Compression.ZipFile.ExtractToDirectory(ArchiveFilePath, ArchiveDirPath);

            File.Delete(ArchiveFilePath);
        }

        public string GetMimeTypeForFileExtension(string filePath)
        {
            const string DefaultContentType = "application/octet-stream";

            var provider = new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(filePath, out string? contentType))
            {
                contentType = DefaultContentType;
            }

            return contentType;
        }

        public record class ProductRecord(string StoreIdOrHandle, string Sku, string Name, string? Handle, string? Description, string? Brand, long? CategoryId, string? CategoryName, string? ParentSku, string? Image, decimal Price, decimal? RegularPrice, bool? Listed);
    }
}
using System.Collections.Generic;
using System.IO;
using System.Text;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

using YourBrand.Catalog.Domain.Entities;
using YourBrand.Catalog.Features.ProductManagement.Import;
using YourBrand.Catalog.Features.ProductManagement.Products.Images;
using YourBrand.Catalog.Persistence;
using YourBrand.Domain;
using YourBrand.Tenancy;

namespace YourBrand.Catalog.UnitTests;

public class ImportProductsTests
{
    [Fact]
    public async Task Imports_products_and_uploads_images()
    {
        // Arrange
        var tenantId = new TenantId("tenant-1");
        var organizationId = new OrganizationId("org-1");

        var tenantContext = new TestTenantContext { TenantId = tenantId };

        var options = new DbContextOptionsBuilder<CatalogContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new CatalogContext(options, tenantContext, NullLogger<CatalogContext>.Instance);

        var currency = new Currency("USD", "US Dollar", "$");
        context.Currencies.Add(currency);

        var store = new Store("Main store", "store-1", currency)
        {
            TenantId = tenantId,
            OrganizationId = organizationId
        };

        context.Stores.Add(store);

        var category = new ProductCategory(1)
        {
            TenantId = tenantId,
            OrganizationId = organizationId,
            Store = store,
            StoreId = store.Id,
            Name = "Category",
            Handle = "category",
            Path = "category",
            CanAddProducts = true
        };

        store.AddCategory(category);
        context.ProductCategories.Add(category);

        var brand = new Brand("Acme", "acme")
        {
            TenantId = tenantId,
            OrganizationId = organizationId
        };

        context.Brands.Add(brand);

        await context.SaveChangesAsync();

        var csv = "StoreIdOrHandle,Sku,Name,Handle,Description,Brand,CategoryId,CategoryName,ParentSku,Image,Price,RegularPrice,Listed\n" +
                  "store-1,SKU-1,Imported Product,imported-product,Description,acme,1,Category,,image.jpg,9.99,10.99,true\n";

        var imageContent = Encoding.UTF8.GetBytes("image-bytes");

        var archiveManager = new InMemoryProductImportArchiveManager(new Dictionary<string, byte[]>
        {
            ["products.csv"] = Encoding.UTF8.GetBytes(csv),
            ["image.jpg"] = imageContent
        });

        var imageUploader = new FakeProductImageUploader();

        var handler = new ImportProducts.Handler(context, imageUploader, archiveManager);

        using var archiveStream = new MemoryStream(Encoding.UTF8.GetBytes("ignored"));
        var request = new ImportProducts(organizationId, archiveStream);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.GetValue().Diagnostics);

        var product = await context.Products.Include(p => p.Images).SingleAsync();

        Assert.Equal("SKU-1", product.Sku);
        Assert.Equal(store.Id, product.StoreId);
        Assert.Equal(organizationId, product.OrganizationId);
        Assert.Equal(brand.Id, product.BrandId);
        Assert.Equal(9.99m, product.Price);
        Assert.Single(product.Images);

        var upload = Assert.Single(imageUploader.Uploads);
        Assert.Equal(product.Id, upload.ProductId);
        Assert.Equal("image.jpg", upload.FileName);
        Assert.Equal("image/jpeg", upload.ContentType);
        Assert.Equal(imageContent, upload.Content);

        var deleteRequest = Assert.Single(imageUploader.DeletedImages);
        Assert.Equal(product.Id, deleteRequest.ProductId);
        Assert.Equal("image.jpg", deleteRequest.FileName);
    }

    private sealed class TestTenantContext : ITenantContext
    {
        public TenantId? TenantId { get; set; }
    }

    private sealed class InMemoryProductImportArchiveManager(Dictionary<string, byte[]> files)
        : IProductImportArchiveManager
    {
        public Task<IProductImportArchive> CreateArchive(Stream stream, CancellationToken cancellationToken)
        {
            IProductImportArchive archive = new InMemoryProductImportArchive(new Dictionary<string, byte[]>(files));
            return Task.FromResult(archive);
        }

        private sealed class InMemoryProductImportArchive(Dictionary<string, byte[]> files)
            : IProductImportArchive
        {
            public Task<Stream> OpenFileAsync(string relativePath, CancellationToken cancellationToken)
            {
                if (!files.TryGetValue(relativePath, out var content))
                {
                    throw new FileNotFoundException(relativePath);
                }

                return Task.FromResult<Stream>(new MemoryStream(content, writable: false));
            }

            public Task DeleteFileAsync(string relativePath, CancellationToken cancellationToken)
            {
                files.Remove(relativePath);
                return Task.CompletedTask;
            }

            public ValueTask DisposeAsync() => ValueTask.CompletedTask;
        }
    }

    private sealed class FakeProductImageUploader : IProductImageUploader
    {
        public List<(int ProductId, string FileName, string ContentType, byte[] Content)> Uploads { get; } = new();

        public List<(int ProductId, string FileName)> DeletedImages { get; } = new();

        public Task<string> GetPlaceholderImageUrl() => Task.FromResult("placeholder");

        public Task<bool> TryDeleteProductImage(int productId, string fileName)
        {
            DeletedImages.Add((productId, fileName));
            return Task.FromResult(true);
        }

        public async Task<string> UploadProductImage(int productId, string fileName, Stream stream, string contentType)
        {
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);

            Uploads.Add((productId, fileName, contentType, memoryStream.ToArray()));

            return $"https://example.com/products/{productId}/{fileName}";
        }
    }
}

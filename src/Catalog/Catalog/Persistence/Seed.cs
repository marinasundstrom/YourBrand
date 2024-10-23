using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain.Entities;
using YourBrand.Catalog.Features.ProductManagement;
using YourBrand.Tenancy;

namespace YourBrand.Catalog.Persistence;

public static class Seed
{
    public static async Task SeedData(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        using CatalogContext context = serviceProvider.GetRequiredService<CatalogContext>();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var tenantContext = serviceProvider.GetRequiredService<ITenantContext>();

        var productFactory = new ProductFactory(context, tenantContext);
        var productCategoryFactory = new ProductCategoryFactory(context, tenantContext);

        var connectionString = context.Database.GetConnectionString()!;

        string cdnBaseUrl = (connectionString.Contains("localhost") || connectionString.Contains("mssql"))
            ? configuration["CdnBaseUrl"]!
            : "https://yourbrandstorage.blob.core.windows.net";

        ProductImage? image;

        context.Set<VatRate>().AddRange(
            new VatRate("25%", 0.25, 1.25, 0.8),
            new VatRate("12%", 0.12, 1.12, 0.89),
            new VatRate("6%", 0.06, 1.06, 0.94));

        await context.SaveChangesAsync();

        var pastries = new ProductCategory()
        {
            OrganizationId = TenantConstants.OrganizationId,
            Name = "Pastries",
            Description = "Try some of our tasty pastries.",
            Handle = "pastries",
            Path = "pastries",
            CanAddProducts = true
        };

        context.ProductCategories.Add(pastries);

        var drinks = new ProductCategory()
        {
            OrganizationId = TenantConstants.OrganizationId,
            Name = "Drinks",
            Description = "All of our drinks.",
            Handle = "drinks",
            Path = "drinks"
        };

        context.ProductCategories.Add(drinks);

        var coffee = new ProductCategory()
        {
            OrganizationId = TenantConstants.OrganizationId,
            Name = "Coffee",
            Description = "Enjoy your favorite coffee.",
            Handle = "coffee",
            CanAddProducts = true
        };

        drinks.AddSubCategory(coffee);

        context.ProductCategories.Add(coffee);

        var otherDrinks = new ProductCategory()
        {
            OrganizationId = TenantConstants.OrganizationId,
            Name = "Other drinks",
            Description = "Try some of our special drinks.",
            Handle = "other",
            CanAddProducts = true
        };

        drinks.AddSubCategory(otherDrinks);

        context.ProductCategories.Add(otherDrinks);

        await context.SaveChangesAsync();

        var biscotti = await productFactory.CreateProductAsync(TenantConstants.OrganizationId, new()
        {
            Name = "Biscotti",
            Handle = "biscotti",
            Price = 10
        }, cancellationToken);

        image = new ProductImage("Biscotti", string.Empty, $"{cdnBaseUrl}/images/products/biscotti.jpeg");
        image.OrganizationId = TenantConstants.OrganizationId;
        biscotti.AddImage(image);
        biscotti.Image = image;

        pastries.AddProduct(biscotti);

        var signatureBrewed = await productFactory.CreateProductAsync(TenantConstants.OrganizationId, new()
        {
            Name = "Signature Brew",
            Description = "Freshly brewed coffee",
            Price = 32,
            Handle = "brewed-coffe"
        }, cancellationToken);

        image = new ProductImage("Signature Brew", string.Empty, $"{cdnBaseUrl}/images/products/coffee.jpeg");
        image.OrganizationId = TenantConstants.OrganizationId;
        signatureBrewed.AddImage(image);
        signatureBrewed.Image = image;

        coffee.AddProduct(signatureBrewed);

        var caffeLatte = await productFactory.CreateProductAsync(TenantConstants.OrganizationId, new()
        {
            Name = "Caffe Latte",
            Description = "Freshly ground espresso coffee with steamed milk",
            DiscountPrice = 32,
            Price = 42,
            Handle = "caffe-latte"
        }, cancellationToken);

        image = new ProductImage("Caffe Latte", string.Empty, $"{cdnBaseUrl}/images/products/caffe-latte.jpeg");
        image.OrganizationId = TenantConstants.OrganizationId;
        caffeLatte.AddImage(image);
        caffeLatte.Image = image;

        coffee.AddProduct(caffeLatte);

        var cinnamonRoll = await productFactory.CreateProductAsync(TenantConstants.OrganizationId, new()
        {
            Name = "Cinnamon roll",
            Description = "Newly baked cinnamon rolls",
            Price = 22,
            Handle = "cinnamon-roll"
        }, cancellationToken);

        image = new ProductImage("Cinnamon Roll", string.Empty, $"{cdnBaseUrl}/images/products/cinnamon-roll.jpeg");
        image.OrganizationId = TenantConstants.OrganizationId;
        cinnamonRoll.AddImage(image);
        cinnamonRoll.Image = image;

        pastries.AddProduct(cinnamonRoll);

        var espresso = await productFactory.CreateProductAsync(TenantConstants.OrganizationId, new()
        {
            Name = "Espresso",
            Description = "Single shot espresso",
            Price = 32,
            Handle = "espresso"
        }, cancellationToken);

        image = new ProductImage("Espresso", string.Empty, $"{cdnBaseUrl}/images/products/espresso.jpeg");
        image.OrganizationId = TenantConstants.OrganizationId;
        espresso.AddImage(image);
        espresso.Image = image;

        coffee.AddProduct(espresso);

        var milkshake = await productFactory.CreateProductAsync(TenantConstants.OrganizationId, new()
        {
            Name = "Milkshake",
            Description = "Our fabulous milkshake",
            Price = 52,
            Handle = "milkshake"
        }, cancellationToken);

        image = new ProductImage("Milkshake", string.Empty, $"{cdnBaseUrl}/images/products/milkshake.jpeg");
        image.OrganizationId = TenantConstants.OrganizationId;
        milkshake.AddImage(image);
        milkshake.Image = image;

        otherDrinks.AddProduct(milkshake);

        var moccaLatte = await productFactory.CreateProductAsync(TenantConstants.OrganizationId, new()
        {
            Name = "Mocca Latte",
            Description = "Caffe Latte with chocolate syrup",
            Price = 32,
            Handle = "mocca-latte"
        }, cancellationToken);

        image = new ProductImage("Mocca Latter", string.Empty, $"{cdnBaseUrl}/images/products/mocca-latte.jpeg");
        image.OrganizationId = TenantConstants.OrganizationId;
        moccaLatte.AddImage(image);
        moccaLatte.Image = image;

        coffee.AddProduct(moccaLatte);

        var applePie = await productFactory.CreateProductAsync(TenantConstants.OrganizationId, new()
        {
            Name = "Apple Pie",
            Description = "Test",
            Price = 15,
            Handle = "apple-pie"
        }, cancellationToken);

        image = new ProductImage("Apple Pie", string.Empty, $"{cdnBaseUrl}/images/products/apple-pie.jpeg");
        image.OrganizationId = TenantConstants.OrganizationId;
        applePie.AddImage(image);
        applePie.Image = image;

        pastries.AddProduct(applePie);

        await context.SaveChangesAsync();
    }
}
using YourBrand.Catalog.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Catalog.Persistence;

public static class Seed
{
    public static async Task SeedData(CatalogContext context, IConfiguration configuration)
    {
        //await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

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
            Name = "Pastries",
            Description = "Try some of our tasty pastries.",
            Handle = "pastries",
            Path = "pastries",
            CanAddProducts = true
        };

        context.ProductCategories.Add(pastries);

        var drinks = new ProductCategory()
        {
            Name = "Drinks",
            Description = "All of our drinks.",
            Handle = "drinks",
            Path = "drinks"
        };

        context.ProductCategories.Add(drinks);

        var coffee = new ProductCategory()
        {
            Name = "Coffee",
            Description = "Enjoy your favorite coffee.",
            Handle = "coffee",
            CanAddProducts = true
        };

        drinks.AddSubCategory(coffee);

        context.ProductCategories.Add(coffee);

        var otherDrinks = new ProductCategory()
        {
            Name = "Other drinks",
            Description = "Try some of our special drinks.",
            Handle = "other",
            CanAddProducts = true
        };

        drinks.AddSubCategory(otherDrinks);

        context.ProductCategories.Add(otherDrinks);

        await context.SaveChangesAsync();

        var biscotti = new Product()
        {
            Name = "Biscotti",
            Description = "Small biscuit",
            Price = 10,
            RegularPrice = null,
            Handle = "biscotti"
        };

        image = new ProductImage("Biscotti", string.Empty, $"{cdnBaseUrl}/images/products/biscotti.jpeg");
        biscotti.AddImage(image);
        biscotti.Image = image;

        pastries.AddProduct(biscotti);

        context.Products.Add(biscotti);

        var signatureBrewed = new Product()
        {
            Name = "Signature Brew",
            Description = "Freshly brewed coffee",
            Price = 32,
            RegularPrice = null,
            Handle = "brewed-coffe"
        };

        image = new ProductImage("Signature Brew", string.Empty, $"{cdnBaseUrl}/images/products/coffee.jpeg");
        signatureBrewed.AddImage(image);
        signatureBrewed.Image = image;

        coffee.AddProduct(signatureBrewed);

        context.Products.Add(signatureBrewed);

        var caffeLatte = new Product()
        {
            Name = "Caffe Latte",
            Description = "Freshly ground espresso coffee with steamed milk",
            Price = 32,
            RegularPrice = 42,
            Handle = "caffe-latte"
        };

        image = new ProductImage("Caffe Latte", string.Empty, $"{cdnBaseUrl}/images/products/caffe-latte.jpeg");
        caffeLatte.AddImage(image);
        caffeLatte.Image = image;

        coffee.AddProduct(caffeLatte);

        context.Products.Add(caffeLatte);

        var cinnamonRoll = new Product()
        {
            Name = "Cinnamon roll",
            Description = "Newly baked cinnamon rolls",
            Price = 22,
            RegularPrice = null,
            Handle = "cinnamon-roll"
        };

        image = new ProductImage("Cinnamon Roll", string.Empty, $"{cdnBaseUrl}/images/products/cinnamon-roll.jpeg");
        cinnamonRoll.AddImage(image);
        cinnamonRoll.Image = image;

        pastries.AddProduct(cinnamonRoll);

        context.Products.Add(cinnamonRoll);

        var espresso = new Product()
        {
            Name = "Espresso",
            Description = "Single shot espresso",
            Price = 32,
            RegularPrice = null,
            Handle = "espresso"
        };

        image = new ProductImage("Espresso", string.Empty, $"{cdnBaseUrl}/images/products/espresso.jpeg");
        espresso.AddImage(image);
        espresso.Image = image;

        coffee.AddProduct(espresso);

        context.Products.Add(espresso);

        var milkshake = new Product()
        {
            Name = "Milkshake",
            Description = "Our fabulous milkshake",
            Price = 52,
            RegularPrice = null,
            Handle = "milkshake"
        };

        image = new ProductImage("Milkshake", string.Empty, $"{cdnBaseUrl}/images/products/milkshake.jpeg");
        milkshake.AddImage(image);
        milkshake.Image = image;

        otherDrinks.AddProduct(milkshake);

        context.Products.Add(milkshake);

        var moccaLatte = new Product()
        {
            Name = "Mocca Latte",
            Description = "Caffe Latte with chocolate syrup",
            Price = 32,
            RegularPrice = null,
            Handle = "mocca-latte"
        };

        image = new ProductImage("Mocca Latter", string.Empty, $"{cdnBaseUrl}/images/products/mocca-latte.jpeg");
        moccaLatte.AddImage(image);
        moccaLatte.Image = image;

        coffee.AddProduct(moccaLatte);

        context.Products.Add(moccaLatte);

        var applePie = new Product()
        {
            Name = "Apple Pie",
            Description = "Test",
            Price = 15,
            RegularPrice = null,
            Handle = "apple-pie"
        };

        image = new ProductImage("Apple Pie", string.Empty, $"{cdnBaseUrl}/images/products/apple-pie.jpeg");
        applePie.AddImage(image);
        applePie.Image = image;

        pastries.AddProduct(applePie);

        context.Products.Add(applePie);

        await context.SaveChangesAsync();
    }
}
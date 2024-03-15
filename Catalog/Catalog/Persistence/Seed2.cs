using YourBrand.Catalog.Domain.Entities;
using YourBrand.Catalog.Domain.Enums;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Catalog.Persistence;

public static class Seed2
{
    private static string cdnBaseUrl;
    private static ProductCategory? drinks;
    private static ProductCategory? food;
    private static ProductCategory? tshirts;
    private static ProductCategory? clothes;

    private static ProductImage? PlaceholderImage;

    public static async Task SeedData(CatalogContext context, IConfiguration configuration)
    {
        //await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        var connectionString = context.Database.GetConnectionString()!;

        cdnBaseUrl = (connectionString.Contains("localhost") || connectionString.Contains("mssql"))
            ? "https://localhost:5120/images/{0}"
            : "https://yourbrandstorage.blob.core.windows.net/images/{0}";

        PlaceholderImage = new ProductImage("Placeholder", string.Empty, string.Format(cdnBaseUrl, "placeholder.jpeg"));

        context.ProductImages.Add(PlaceholderImage);

        var currency = await context.Currencies.FirstOrDefaultAsync(x => x.Code == "SEK");

        if (currency is null)
        {
            context.Currencies.Add(currency ??= new Currency("SEK", "Swedish Krona", "kr"));
        }

        await context.SaveChangesAsync();

        var store = await context.Stores.FirstOrDefaultAsync(x => x.Handle == "my-store");

        if (store is null)
        {
            var currency2 = await context.Currencies.FirstAsync(x => x.Code == "SEK");

            var myStore = new Store("My store", "my-store", currency2);
            myStore.CurrencyDisplayOptions = new CurrencyDisplayOptions
            {
                IncludeVatInSalesPrice = true
            };

            context.Stores.Add(store ??= myStore);
        }

        var brand = await context.Brands.FirstOrDefaultAsync(x => x.Handle == "my-brand");

        if (brand is null)
        {
            context.Brands.Add(brand ??= new Brand("My brand", "my-brand"));
        }

        context.Set<VatRate>().AddRange(
            new VatRate("25%", 0.25, 1.25, 0.8),
            new VatRate("12%", 0.12, 1.12, 0.89),
            new VatRate("6%", 0.06, 1.06, 0.94));

        await context.SaveChangesAsync();

        clothes = await context.ProductCategories.FirstOrDefaultAsync(x => x.Handle == "clothes");

        if (clothes is null)
        {
            context.ProductCategories.Add(clothes ??= new ProductCategory("Clothes")
            {
                Handle = "clothes",
                Path = "clothes",
                Description = null,
                Store = await context.Stores.FirstAsync(x => x.Handle == "my-store")
            });
        }

        tshirts = await context.ProductCategories.FirstOrDefaultAsync(x => x.Handle == "t-shirts");

        if (tshirts is null)
        {
            context.ProductCategories.Add(tshirts ??= new ProductCategory("T-shirts")
            {
                Handle = "t-shirts",
                Path = "t-shirts",
                Description = null,
                CanAddProducts = true,
                Store = await context.Stores.FirstAsync(x => x.Handle == "my-store"),
            });

            clothes.AddSubCategory(tshirts);
        }

        food = await context.ProductCategories.FirstOrDefaultAsync(x => x.Handle == "food");

        if (food is null)
        {
            context.ProductCategories.Add(food ??= new ProductCategory("Food")
            {
                Handle = "food",
                Path = "food",
                Description = null,
                CanAddProducts = true,
                Store = await context.Stores.FirstAsync(x => x.Handle == "my-store")
            });
        }

        drinks = await context.ProductCategories.FirstOrDefaultAsync(x => x.Handle == "drinks");

        if (drinks is null)
        {
            context.ProductCategories.Add(food ??= new ProductCategory("Drinks")
            {
                Handle = "drinks",
                Path = "drinks",
                Description = null,
                CanAddProducts = true,
                Store = await context.Stores.FirstAsync(x => x.Handle == "my-store")
            });
        }

        await context.SaveChangesAsync();

        await CreateTShirt(context);

        await CreateKebabPlate(context);

        await CreateHerrgardsStek(context);

        await CreateKorg(context);

        await CreatePizza(context);

        await CreateSalad(context);
    }

    public static async Task CreateTShirt(CatalogContext context)
    {
        var sizeAttribute = new Domain.Entities.Attribute("Size");

        context.Attributes.Add(sizeAttribute);
        var valueSmall = new AttributeValue("Small");

        sizeAttribute.Values.Add(valueSmall);
        var valueMedium = new AttributeValue("Medium");

        sizeAttribute.Values.Add(valueMedium);
        var valueLarge = new AttributeValue("Large");

        sizeAttribute.Values.Add(valueLarge);
        context.Attributes.Add(sizeAttribute);

        var colorAttribute = new Domain.Entities.Attribute("Color");
        context.Attributes.Add(colorAttribute);

        var valueBlue = new AttributeValue("Blue");
        colorAttribute.Values.Add(valueBlue);

        var valueRed = new AttributeValue("Red");
        colorAttribute.Values.Add(valueRed);

        var product = new Product("Färgad t-shirt", "fargad-tshirt")
        {
            Description = "",
            Headline = "T-shirt i olika färger",
            HasVariants = true,
            ListingState = ProductListingState.Listed,
            Brand = await context.Brands.FirstAsync(x => x.Handle == "my-brand"),
            Store = await context.Stores.FirstAsync(x => x.Handle == "my-store"),
            Image = PlaceholderImage
        };

        tshirts.AddProduct(product);

        context.Products.Add(product);

        product.AddProductAttribute(new ProductAttribute
        {
            ForVariant = true,
            IsMainAttribute = true,
            Attribute = colorAttribute,
            Value = null
        });

        product.AddProductAttribute(new ProductAttribute
        {
            ForVariant = true,
            IsMainAttribute = false,
            Attribute = sizeAttribute,
            Value = null
        });

        await context.SaveChangesAsync();

        var variantBlueSmall = new Product("Blue S", "tshirt-blue-small")
        {
            Description = "",
            Gtin = "4345547457457",
            Price = 120,
            VatRate = 0.25,
            VatRateId = (await context.VatRates.FirstOrDefaultAsync(x => x.Rate == 0.25))?.Id,
            Store = await context.Stores.FirstAsync(x => x.Handle == "my-store"),
            Image = PlaceholderImage,
        };

        variantBlueSmall.AddProductAttribute(new ProductAttribute
        {
            Attribute = sizeAttribute,
            Value = valueSmall,
            ForVariant = true
        });

        variantBlueSmall.AddProductAttribute(new ProductAttribute
        {
            Attribute = colorAttribute,
            Value = valueBlue,
            ForVariant = true,
            IsMainAttribute = true
        });

        product.AddVariant(variantBlueSmall);

        //*/

        var variantBlueMedium = new Product("Blue M", "tshirt-blue-medium")
        {
            Description = "",
            Gtin = "543453454567",
            Price = 120,
            VatRate = 0.25,
            VatRateId = (await context.VatRates.FirstOrDefaultAsync(x => x.Rate == 0.25))?.Id,
            Store = await context.Stores.FirstAsync(x => x.Handle == "my-store"),
            Image = PlaceholderImage,
        };

        variantBlueMedium.AddProductAttribute(new ProductAttribute
        {
            Attribute = sizeAttribute,
            Value = valueMedium,
            ForVariant = true
        });

        variantBlueMedium.AddProductAttribute(new ProductAttribute
        {
            Attribute = colorAttribute,
            Value = valueBlue,
            ForVariant = true,
            IsMainAttribute = true
        });

        product.AddVariant(variantBlueMedium);

        var variantBlueLarge = new Product("Blue L", "tshirt-blue-large")
        {
            Description = "",
            Gtin = "6876345345345",
            Price = 60,
            VatRate = 0.25,
            VatRateId = (await context.VatRates.FirstOrDefaultAsync(x => x.Rate == 0.25))?.Id,
            Store = await context.Stores.FirstAsync(x => x.Handle == "my-store"),
            Image = PlaceholderImage,
        };

        variantBlueLarge.AddProductAttribute(new ProductAttribute
        {
            Attribute = sizeAttribute,
            Value = valueLarge,
            ForVariant = true
        });

        variantBlueLarge.AddProductAttribute(new ProductAttribute
        {
            Attribute = colorAttribute,
            Value = valueBlue,
            ForVariant = true,
            IsMainAttribute = true
        });

        product.AddVariant(variantBlueLarge);

        /////

        var variantRedSmall = new Product("Red S", "tshirt-red-small")
        {
            Description = "",
            Gtin = "4345547457457",
            Price = 120,
            VatRate = 0.25,
            VatRateId = (await context.VatRates.FirstOrDefaultAsync(x => x.Rate == 0.25))?.Id,
            Store = await context.Stores.FirstAsync(x => x.Handle == "my-store"),
            Image = PlaceholderImage,
        };

        variantRedSmall.AddProductAttribute(new ProductAttribute
        {
            Attribute = sizeAttribute,
            Value = valueSmall,
            ForVariant = true
        });

        variantRedSmall.AddProductAttribute(new ProductAttribute
        {
            Attribute = colorAttribute,
            Value = valueRed,
            ForVariant = true,
            IsMainAttribute = true
        });

        product.AddVariant(variantRedSmall);

        var variantRedMedium = new Product("Red M", "tshirt-red-medium")
        {
            Description = "",
            Gtin = "543453454567",
            Price = 120,
            VatRate = 0.25,
            VatRateId = (await context.VatRates.FirstOrDefaultAsync(x => x.Rate == 0.25))?.Id,
            Store = await context.Stores.FirstAsync(x => x.Handle == "my-store"),
            Image = PlaceholderImage,
        };

        variantRedMedium.AddProductAttribute(new ProductAttribute
        {
            Attribute = sizeAttribute,
            Value = valueMedium,
            ForVariant = true,
        });

        variantRedMedium.AddProductAttribute(new ProductAttribute
        {
            Attribute = colorAttribute,
            Value = valueRed,
            ForVariant = true,
            IsMainAttribute = true
        });

        product.AddVariant(variantRedMedium);

        var variantRedLarge = new Product("Red L", "tshirt-red-large")
        {
            Description = "",
            Gtin = "6876345345345",
            Price = 120,
            VatRate = 0.25,
            VatRateId = (await context.VatRates.FirstOrDefaultAsync(x => x.Rate == 0.25))?.Id,
            Store = await context.Stores.FirstAsync(x => x.Handle == "my-store"),
            Image = PlaceholderImage,
        };

        variantRedLarge.AddProductAttribute(new ProductAttribute
        {
            Attribute = sizeAttribute,
            Value = valueLarge,
            ForVariant = true
        });

        variantRedLarge.AddProductAttribute(new ProductAttribute
        {
            Attribute = colorAttribute,
            Value = valueRed,
            ForVariant = true,
            IsMainAttribute = true
        });

        product.AddVariant(variantRedLarge);

        var textOption = new Domain.Entities.TextValueOption("Custom text");

        product.AddOption(textOption);

        await context.SaveChangesAsync();
    }

    public static async Task CreateKebabPlate(CatalogContext context)
    {
        var product = new Product("Kebabtallrik", "kebabtallrik")
        {
            Description = "",
            Headline = "Dönnerkebab, nyfriterad pommes frites, sallad, och sås",
            Price = 89,
            VatRate = 0.12,
            VatRateId = (await context.VatRates.FirstOrDefaultAsync(x => x.Rate == 0.12))?.Id,
            Store = await context.Stores.FirstAsync(x => x.Handle == "my-store"),
            Image = PlaceholderImage,
        };

        food.AddProduct(product);

        context.Products.Add(product);

        await context.SaveChangesAsync();

        var option = new ChoiceOption("Sås");
        product.AddOption(option);

        await context.SaveChangesAsync();

        var valueSmall = new OptionValue("Mild sås");

        option.Values.Add(valueSmall);

        var valueMedium = new OptionValue("Stark sås");

        option.Values.Add(valueMedium);

        var valueLarge = new OptionValue("Blandad sås");

        option.DefaultValue = valueSmall;

        option.Values.Add(valueLarge);

        await context.SaveChangesAsync();
    }

    public static async Task CreateHerrgardsStek(CatalogContext context)
    {
        var product = new Product("Herrgårdsstek", "herrgardsstek")
        {
            Description = "",
            Headline = "Vår fina stek med pommes och vår hemlagade bearnaise sås",
            Price = 179,
            VatRate = 0.12,
            VatRateId = (await context.VatRates.FirstOrDefaultAsync(x => x.Rate == 0.12))?.Id,
            Store = await context.Stores.FirstAsync(x => x.Handle == "my-store"),
            Image = PlaceholderImage,
        };

        food.AddProduct(product);

        context.Products.Add(product);

        await context.SaveChangesAsync();

        var optionDoneness = new ChoiceOption("Stekning");

        product.AddOption(optionDoneness);

        await context.SaveChangesAsync();

        optionDoneness.Values.Add(new OptionValue("Rare")
        {
            Seq = 1
        });

        var optionMediumRare = new OptionValue("Medium Rare")
        {
            Seq = 2
        };

        optionDoneness.Values.Add(optionMediumRare);

        optionDoneness.Values.Add(new OptionValue("Well Done")
        {
            Seq = 3
        });

        optionDoneness.DefaultValue = optionMediumRare;

        var optionSize = new SelectableOption("Extra stor - 50 g mer")
        {
            Price = 15
        };

        product.AddOption(optionSize);

        await context.SaveChangesAsync();
    }

    public static async Task CreateKorg(CatalogContext context)
    {
        var product = new Product("Korg", "korg")
        {
            Description = "",
            Headline = "En korg med smårätter",
            Price = 179,
            VatRate = 0.12,
            VatRateId = (await context.VatRates.FirstOrDefaultAsync(x => x.Rate == 0.12))?.Id,
            Store = await context.Stores.FirstAsync(x => x.Handle == "my-store"),
            Image = PlaceholderImage,
        };

        food.AddProduct(product);

        context.Products.Add(product);

        await context.SaveChangesAsync();

        var ratterGroup = new OptionGroup("Rätter")
        {
            Max = 7
        };

        product.AddOptionGroup(ratterGroup);

        var optionFalafel = new NumericalValueOption("Falafel")
        {
            Group = ratterGroup
        };

        product.AddOption(optionFalafel);

        var optionChickenWing = new NumericalValueOption("Spicy Chicken Wing")
        {
            Group = ratterGroup
        };

        product.AddOption(optionChickenWing);

        var optionRib = new NumericalValueOption("Rib")
        {
            Group = ratterGroup
        };

        product.AddOption(optionRib);

        await context.SaveChangesAsync();


        var extraGroup = new OptionGroup("Extra");

        product.AddOptionGroup(extraGroup);

        await context.SaveChangesAsync();

        var optionSauce = new ChoiceOption("Sås")
        {
            Group = extraGroup
        };

        product.AddOption(optionSauce);

        optionSauce.Values.Add(new OptionValue("Favoritsås")
        {
            Price = 10
        });

        optionSauce.Values.Add(new OptionValue("Barbecuesås")
        {
            Price = 10
        });

        await context.SaveChangesAsync();
    }

    public static async Task CreatePizza(CatalogContext context)
    {
        var product = new Product("Pizza", "pizza")
        {
            Description = "",
            Headline = "Custom pizza",
            Price = 40,
            VatRate = 0.12,
            VatRateId = (await context.VatRates.FirstOrDefaultAsync(x => x.Rate == 0.12))?.Id,
            Store = await context.Stores.FirstAsync(x => x.Handle == "my-store"),
            Image = PlaceholderImage,
        };

        food.AddProduct(product);

        context.Products.Add(product);

        await context.SaveChangesAsync();

        var breadGroup = new OptionGroup("Meat")
        {
            Seq = 1,
        };

        product.AddOptionGroup(breadGroup);

        var meatGroup = new OptionGroup("Meat")
        {
            Seq = 2,
            Max = 2
        };

        product.AddOptionGroup(meatGroup);

        var nonMeatGroup = new OptionGroup("Non-Meat")
        {
            Seq = 3
        };

        product.AddOptionGroup(nonMeatGroup);

        var sauceGroup = new OptionGroup("Sauce")
        {
            Seq = 4
        };

        product.AddOptionGroup(sauceGroup);

        var toppingsGroup = new OptionGroup("Toppings")
        {
            Seq = 5
        };

        product.AddOptionGroup(toppingsGroup);

        await context.SaveChangesAsync();

        var optionStyle = new ChoiceOption("Style");

        product.AddOption(optionStyle);

        await context.SaveChangesAsync();

        var valueItalian = new OptionValue("Italian");

        optionStyle.Values.Add(valueItalian);

        var valueAmerican = new OptionValue("American");

        optionStyle.DefaultValue = valueAmerican;

        optionStyle.Values.Add(valueAmerican);

        var optionHam = new SelectableOption("Ham")
        {
            Group = meatGroup,
            Price = 15
        };

        product.AddOption(optionHam);

        var optionKebab = new SelectableOption("Kebab")
        {
            Group = meatGroup,
            Price = 10,
            IsSelected = true
        };

        product.AddOption(optionKebab);

        var optionChicken = new SelectableOption("Chicken")
        {
            Group = meatGroup,
            Price = 10
        };

        product.AddOption(optionChicken);

        var optionExtraCheese = new SelectableOption("Extra cheese")
        {
            Group = toppingsGroup,
            Price = 5
        };

        product.AddOption(optionExtraCheese);

        var optionGreenOlives = new SelectableOption("Green Olives")
        {
            Group = toppingsGroup,
            Price = 5
        };

        product.AddOption(optionGreenOlives);

        await context.SaveChangesAsync();
    }

    public static async Task CreateSalad(CatalogContext context)
    {
        var product = new Product("Sallad", "sallad")
        {
            Description = "",
            Headline = "Din egna sallad",
            Price = 52,
            VatRate = 0.12,
            VatRateId = (await context.VatRates.FirstOrDefaultAsync(x => x.Rate == 0.12))?.Id,
            ListingState = ProductListingState.Listed,
            Store = await context.Stores.FirstAsync(x => x.Handle == "my-store"),
            Image = PlaceholderImage,
        };

        product.SetPrice(52);

        food.AddProduct(product);

        context.Products.Add(product);

        var baseGroup = new OptionGroup("Bas")
        {
            Seq = 1,
        };

        product.AddOptionGroup(baseGroup);

        var proteinGroup = new OptionGroup("Välj protein")
        {
            Seq = 2,
            Max = 1
        };

        product.AddOptionGroup(proteinGroup);

        var additionalGroup = new OptionGroup("Välj tillbehör")
        {
            Seq = 4,
            Max = 3
        };

        product.AddOptionGroup(additionalGroup);

        var dressingGroup = new OptionGroup("Välj dressing")
        {
            Seq = 5,
            Max = 1
        };

        product.AddOptionGroup(dressingGroup);

        await context.SaveChangesAsync();

        var optionBase = new ChoiceOption("Bas")
        {
            Group = baseGroup
        };

        product.AddOption(optionBase);

        await context.SaveChangesAsync();

        var valueSallad = new OptionValue("Sallad");

        optionBase.Values.Add(valueSallad);

        var valueSalladPasta = new OptionValue("Sallad med pasta");

        optionBase.DefaultValue = valueSalladPasta;

        optionBase.Values.Add(valueSalladPasta);

        var valueSalladQuinoa = new OptionValue("Sallad med quinoa");

        optionBase.Values.Add(valueSalladQuinoa);

        var valueSalladNudlar = new OptionValue("Sallad med glasnudlar");

        optionBase.Values.Add(valueSalladNudlar);

        var optionChicken = new SelectableOption("Kycklingfilé")
        {
            Group = proteinGroup
        };

        product.AddOption(optionChicken);

        var optionSmokedTurkey = new SelectableOption("Rökt kalkonfilé")
        {
            Group = proteinGroup
        };

        product.AddOption(optionSmokedTurkey);

        var optionBeanMix = new SelectableOption("Marinerad bönmix")
        {
            Group = proteinGroup
        };

        product.AddOption(optionBeanMix);

        var optionVegMe = new SelectableOption("VegMe")
        {
            Group = proteinGroup
        };

        product.AddOption(optionVegMe);

        var optionChevre = new SelectableOption("Chevré")
        {
            Group = proteinGroup
        };

        product.AddOption(optionChevre);

        var optionSmokedSalmon = new SelectableOption("Varmrökt lax")
        {
            Group = proteinGroup
        };

        product.AddOption(optionSmokedSalmon);

        var optionPrawns = new SelectableOption("Handskalade räkor")
        {
            Group = proteinGroup
        };

        product.AddOption(optionPrawns);

        var optionCheese = new SelectableOption("Parmesanost")
        {
            Group = additionalGroup
        };

        product.AddOption(optionCheese);

        var optionGreenOlives = new SelectableOption("Gröna oliver")
        {
            Group = additionalGroup
        };

        product.AddOption(optionGreenOlives);

        var optionSoltorkadTomat = new SelectableOption("Soltorkade tomater")
        {
            Group = additionalGroup
        };

        product.AddOption(optionSoltorkadTomat);

        var optionInlagdRödlök = new SelectableOption("Inlagd rödlök")
        {
            Group = additionalGroup
        };

        product.AddOption(optionInlagdRödlök);

        var optionRostadAioli = new SelectableOption("Rostad aioli")
        {
            Group = dressingGroup
        };

        product.AddOption(optionRostadAioli);

        var optionPesto = new SelectableOption("Pesto")
        {
            Group = dressingGroup
        };

        product.AddOption(optionPesto);

        var optionOrtvinagret = new SelectableOption("Örtvinägrett")
        {
            Group = dressingGroup
        };

        product.AddOption(optionOrtvinagret);

        var optionSoyavinagret = new SelectableOption("Soyavinägrett")
        {
            Group = dressingGroup
        };

        product.AddOption(optionSoyavinagret);

        var optionRhodeIsland = new SelectableOption("Rhode Island")
        {
            Group = dressingGroup
        };

        product.AddOption(optionRhodeIsland);

        var optionKimchimayo = new SelectableOption("Kimchimayo")
        {
            Group = dressingGroup
        };

        product.AddOption(optionKimchimayo);

        var optionCaesar = new SelectableOption("Caesar")
        {
            Group = dressingGroup
        };

        product.AddOption(optionCaesar);

        var optionCitronLime = new SelectableOption("Citronlime")
        {
            Group = dressingGroup
        };

        product.AddOption(optionCitronLime);

        await context.SaveChangesAsync();
    }
}
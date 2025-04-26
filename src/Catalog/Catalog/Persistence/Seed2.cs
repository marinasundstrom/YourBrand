using System.Text.Json;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain.Entities;
using YourBrand.Catalog.Domain.Enums;
using YourBrand.Catalog.Features.ProductManagement;
using YourBrand.Tenancy;

namespace YourBrand.Catalog.Persistence;

public static class Seed2
{
    private static string cdnBaseUrl;
    private static ProductCategory? drinks;
    private static ProductCategory? food;
    private static ProductCategory? tshirts;
    private static ProductCategory? clothes;
    private static ProductCategory? services;

    private static ProductImage? PlaceholderImage;
    private static Currency? currency;
    private static Brand? brand;
    private static Store? store;

    public static async Task SeedData(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        using CatalogContext context = serviceProvider.GetRequiredService<CatalogContext>();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var tenantContext = serviceProvider.GetRequiredService<ITenantContext>();

        var productFactory = new ProductFactory(context, tenantContext);
        var productCategoryFactory = new ProductCategoryFactory(context, tenantContext);

        var connectionString = context.Database.GetConnectionString()!;

        cdnBaseUrl = (connectionString.Contains("localhost") || connectionString.Contains("mssql"))
            ? "https://localhost:5120/images/{0}"
            : "https://yourbrandstorage.blob.core.windows.net/images/{0}";

        PlaceholderImage = new ProductImage("Placeholder", string.Empty, string.Format(cdnBaseUrl, "placeholder.jpeg"));
        PlaceholderImage.OrganizationId = TenantConstants.OrganizationId;

        context.ProductImages.Add(PlaceholderImage);

        currency = await context.Currencies.FirstOrDefaultAsync(x => x.Code == "SEK");

        if (currency is null)
        {
            context.Currencies.Add(currency ??= new Currency("SEK", "Swedish Krona", "kr"));
        }

        await context.SaveChangesAsync();

        store = await context.Stores.FirstOrDefaultAsync(x => x.Handle == "my-store");

        if (store is null)
        {
            var currency2 = await context.Currencies.FirstAsync(x => x.Code == "SEK");

            var myStore = new Store("My store", "my-store", currency2);
            myStore.OrganizationId = TenantConstants.OrganizationId;
            myStore.CurrencyDisplayOptions = new CurrencyDisplayOptions
            {
                IncludeVatInSalesPrice = true
            };

            context.Stores.Add(store ??= myStore);
        }

        brand = await context.Brands.FirstOrDefaultAsync(x => x.Handle == "my-brand");

        if (brand is null)
        {
            context.Brands.Add(brand ??= new Brand("My brand", "my-brand"));
        }

        brand.OrganizationId = TenantConstants.OrganizationId;

        context.Set<VatRate>().AddRange(
            new VatRate("25%", 0.25, 1.25, 0.8),
            new VatRate("12%", 0.12, 1.12, 0.89),
            new VatRate("6%", 0.06, 1.06, 0.94));

        await context.SaveChangesAsync();

        clothes = await context.ProductCategories.FirstOrDefaultAsync(x => x.Handle == "clothes");

        if (clothes is null)
        {
            clothes ??= await productCategoryFactory.CreateProductCategoryAsync(TenantConstants.OrganizationId, new()
            {
                Name = "Clothes",
                OrganizationId = TenantConstants.OrganizationId,
                Handle = "clothes",
                Path = "clothes",
                Description = null,
                StoreId = store.Id,
            }, cancellationToken);
        }

        tshirts = await context.ProductCategories.FirstOrDefaultAsync(x => x.Handle == "t-shirts");

        if (tshirts is null)
        {
            tshirts ??= await productCategoryFactory.CreateProductCategoryAsync(TenantConstants.OrganizationId, new()
            {
                Name = "T-shirts",
                OrganizationId = TenantConstants.OrganizationId,
                Handle = "t-shirts",
                Path = "t-shirts",
                Description = null,
                CanAddProducts = true,
                StoreId = store.Id
            }, cancellationToken);

            clothes.AddSubCategory(tshirts);
        }

        food = await context.ProductCategories.FirstOrDefaultAsync(x => x.Handle == "food");

        if (food is null)
        {
            food ??= await productCategoryFactory.CreateProductCategoryAsync(TenantConstants.OrganizationId, new()
            {
                Name = "Food",
                OrganizationId = TenantConstants.OrganizationId,
                Handle = "food",
                Path = "food",
                Description = null,
                CanAddProducts = true,
                StoreId = store.Id,
            }, cancellationToken);
        }

        drinks = await context.ProductCategories.FirstOrDefaultAsync(x => x.Handle == "drinks");

        if (drinks is null)
        {
            drinks ??= await productCategoryFactory.CreateProductCategoryAsync(TenantConstants.OrganizationId, new()
            {
                Name = "Drinks",
                OrganizationId = TenantConstants.OrganizationId,
                Handle = "drinks",
                Path = "drinks",
                Description = null,
                CanAddProducts = true,
                StoreId = store.Id,
            }, cancellationToken);
        }

        services = await context.ProductCategories.FirstOrDefaultAsync(x => x.Handle == "services");

        if (services is null)
        {
            services ??= await productCategoryFactory.CreateProductCategoryAsync(TenantConstants.OrganizationId, new()
            {
                Name = "Services",
                OrganizationId = TenantConstants.OrganizationId,
                Handle = "services",
                Path = "services",
                Description = null,
                CanAddProducts = true,
                StoreId = store.Id,
            }, cancellationToken);
        }

        await context.SaveChangesAsync();

        await CreateTShirt(context, productFactory, cancellationToken);

        await CreateKebabPlate(context, productFactory, cancellationToken);

        await CreateHerrgardsStek(context, productFactory, cancellationToken);

        await CreateKorg(context, productFactory, cancellationToken);

        await CreatePizza(context, productFactory, cancellationToken);

        await CreateSallad(context, productFactory, cancellationToken);

        await CreateHamburger(context, productFactory, cancellationToken);

        await CreateCocaCola(context, productFactory, cancellationToken);

        await CreateCocaColaZero(context, productFactory, cancellationToken);

        await CreateCarlsberg(context, productFactory, cancellationToken);

        await CreateHouseCleaningService(context, productFactory, cancellationToken);
    }

    public static async Task CreateTShirt(CatalogContext context, ProductFactory productFactory, CancellationToken cancellationToken = default)
    {
        var sizeAttribute = new Domain.Entities.Attribute("Size");
        sizeAttribute.OrganizationId = TenantConstants.OrganizationId;

        context.Attributes.Add(sizeAttribute);
        var valueSmall = new AttributeValue("Small");

        sizeAttribute.AddValue(valueSmall);
        var valueMedium = new AttributeValue("Medium");

        sizeAttribute.AddValue(valueMedium);
        var valueLarge = new AttributeValue("Large");

        sizeAttribute.AddValue(valueLarge);
        context.Attributes.Add(sizeAttribute);

        var colorAttribute = new Domain.Entities.Attribute("Color");
        colorAttribute.OrganizationId = TenantConstants.OrganizationId;

        context.Attributes.Add(colorAttribute);

        var valueBlue = new AttributeValue("Blue");
        colorAttribute.AddValue(valueBlue);

        var valueRed = new AttributeValue("Red");
        colorAttribute.AddValue(valueRed);

        var product = await productFactory.CreateProductAsync(TenantConstants.OrganizationId, new()
        {
            Name = "T-shirt i färg",
            Handle = "tshirt-fargad",
            Description = "",
            Headline = "T-shirt i olika färger",
            HasVariants = true,
            ListingState = Domain.Enums.ProductListingState.Listed,
            VatRate = 0.25,
            BrandId = brand.Id,
            StoreId = store.Id,
            ImageId = PlaceholderImage.Id
        }, cancellationToken);

        tshirts.AddProduct(product);

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

        var variantBlueSmall = await productFactory.CreateProductAsync(TenantConstants.OrganizationId, new()
        {
            Name = "Blue S",
            Handle = "tshirt-blue-small",
            Description = "",
            Gtin = "4345547457457",
            Price = 120,
            VatRate = 0.25,
            StoreId = store.Id,
            ImageId = PlaceholderImage.Id,
        }, cancellationToken);

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

        var variantBlueMedium = await productFactory.CreateProductAsync(TenantConstants.OrganizationId, new()
        {
            Name = "Blue M",
            Handle = "tshirt-blue-medium",
            Description = "",
            Gtin = "543453454567",
            Price = 120,
            VatRate = 0.25,
            StoreId = store.Id,
            ImageId = PlaceholderImage.Id,
        }, cancellationToken);

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

        var variantBlueLarge = await productFactory.CreateProductAsync(TenantConstants.OrganizationId, new()
        {
            Name = "Blue L",
            Handle = "tshirt-blue-large",
            Description = "",
            Gtin = "6876345345345",
            Price = 60,
            VatRate = 0.25,
            StoreId = store.Id,
            ImageId = PlaceholderImage.Id,
        }, cancellationToken);

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

        var variantRedSmall = await productFactory.CreateProductAsync(TenantConstants.OrganizationId, new()
        {
            Name = "Red S",
            Handle = "tshirt-red-small",
            Description = "",
            Gtin = "4345547457457",
            Price = 120,
            VatRate = 0.25,
            StoreId = store.Id,
            ImageId = PlaceholderImage.Id,
        }, cancellationToken);

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

        var variantRedMedium = await productFactory.CreateProductAsync(TenantConstants.OrganizationId, new()
        {
            Name = "Red M",
            Handle = "tshirt-red-medium",
            Description = "",
            Gtin = "543453454567",
            Price = 120,
            VatRate = 0.25,
            StoreId = store.Id,
            ImageId = PlaceholderImage.Id,
        }, cancellationToken);

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

        var variantRedLarge = await productFactory.CreateProductAsync(TenantConstants.OrganizationId, new()
        {
            Name = "Red L",
            Handle = "tshirt-red-large",
            Description = "",
            Gtin = "6876345345345",
            Price = 120,
            VatRate = 0.25,
            StoreId = store.Id,
            ImageId = PlaceholderImage.Id,
        }, cancellationToken);

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

    public static async Task CreateKebabPlate(CatalogContext context, ProductFactory productFactory, CancellationToken cancellationToken = default)
    {
        var product = await productFactory.CreateProductAsync(TenantConstants.OrganizationId, new()
        {
            Name = "Kebabtallrik",
            Handle = "kebabtallrik",
            Headline = "Dönnerkebab, nyfriterad pommes frites, sallad, och sås",
            Description = "",
            Price = 89,
            VatRate = 0.12,
            StoreId = store.Id,
            ImageId = PlaceholderImage.Id,
        }, cancellationToken);

        food.AddProduct(product);

        await context.SaveChangesAsync();

        var option = new ChoiceOption("Sås");
        product.AddOption(option);

        Console.WriteLine($"test: {JsonSerializer.Serialize(option)}");

        await context.SaveChangesAsync();

        var mildSauce = new OptionValue("Mild sås");

        option.AddValue(mildSauce);

        var hotSauce = new OptionValue("Stark sås");

        option.AddValue(hotSauce);

        var allSauces = new OptionValue("Blandad sås");

        option.DefaultValue = mildSauce;

        option.AddValue(allSauces);

        await context.SaveChangesAsync();
    }

    public static async Task CreateHerrgardsStek(CatalogContext context, ProductFactory productFactory, CancellationToken cancellationToken = default)
    {
        var product = await productFactory.CreateProductAsync(TenantConstants.OrganizationId, new()
        {
            Name = "Herrgårdsstek",
            Handle = "herrgardsstek",
            Headline = "Vår fina stek med pommes och vår hemlagade bearnaise sås",
            Description = "",
            Price = 179,
            VatRate = 0.12,
            StoreId = store.Id,
            ImageId = PlaceholderImage.Id,
        }, cancellationToken);

        food.AddProduct(product);

        await context.SaveChangesAsync();

        var optionDoneness = new ChoiceOption("Stekning");

        product.AddOption(optionDoneness);

        await context.SaveChangesAsync();

        optionDoneness.AddValue(new OptionValue("Rare")
        {
            Seq = 1
        });

        var optionMediumRare = new OptionValue("Medium Rare")
        {
            Seq = 2
        };

        optionDoneness.AddValue(optionMediumRare);

        optionDoneness.AddValue(new OptionValue("Well Done")
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

    public static async Task CreateKorg(CatalogContext context, ProductFactory productFactory, CancellationToken cancellationToken = default)
    {
        var product = await productFactory.CreateProductAsync(TenantConstants.OrganizationId, new()
        {
            Name = "Korg",
            Handle = "korg",
            Headline = "En korg med smårätter",
            Description = "",
            Price = 179,
            VatRate = 0.12,
            StoreId = store.Id,
            ImageId = PlaceholderImage.Id,
        }, cancellationToken);

        food.AddProduct(product);

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

        optionSauce.AddValue(new OptionValue("Favoritsås")
        {
            Price = 10
        });

        optionSauce.AddValue(new OptionValue("Barbecuesås")
        {
            Price = 10
        });

        await context.SaveChangesAsync();
    }

    public static async Task CreatePizza(CatalogContext context, ProductFactory productFactory, CancellationToken cancellationToken = default)
    {
        var product = await productFactory.CreateProductAsync(TenantConstants.OrganizationId, new()
        {
            Name = "Pizza",
            Handle = "pizza",
            Headline = "Custom pizza",
            Description = "",
            Price = 40,
            VatRate = 0.12,
            StoreId = store.Id,
            ImageId = PlaceholderImage.Id,
        }, cancellationToken);

        food.AddProduct(product);

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

        optionStyle.AddValue(valueItalian);

        var valueAmerican = new OptionValue("American");

        optionStyle.DefaultValue = valueAmerican;

        optionStyle.AddValue(valueAmerican);

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

    public static async Task CreateSallad(CatalogContext context, ProductFactory productFactory, CancellationToken cancellationToken = default)
    {
        var product = await productFactory.CreateProductAsync(TenantConstants.OrganizationId, new()
        {
            Name = "Sallad",
            Handle = "sallad",
            Headline = "Din egna sallad",
            Description = "",
            Price = 52,
            VatRate = 0.12,
            StoreId = store.Id,
            ImageId = PlaceholderImage.Id,
            ListingState = Domain.Enums.ProductListingState.Listed
        }, cancellationToken);

        product.SetPrice(52);

        food.AddProduct(product);

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

        optionBase.AddValue(valueSallad);

        var valueSalladPasta = new OptionValue("Sallad med pasta");

        optionBase.DefaultValue = valueSalladPasta;

        optionBase.AddValue(valueSalladPasta);

        var valueSalladQuinoa = new OptionValue("Sallad med quinoa");

        optionBase.AddValue(valueSalladQuinoa);

        var valueSalladNudlar = new OptionValue("Sallad med glasnudlar");

        optionBase.AddValue(valueSalladNudlar);

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


    public static async Task CreateHamburger(CatalogContext context, ProductFactory productFactory, CancellationToken cancellationToken = default)
    {
        var product = await productFactory.CreateProductAsync(TenantConstants.OrganizationId, new()
        {
            Name = "Big Burger",
            Handle = "big-hamburger",
            Headline = "Tasty burger",
            Description = "",
            VatRate = 0.12,
            StoreId = store.Id,
            ImageId = PlaceholderImage.Id,
            ListingState = Domain.Enums.ProductListingState.Listed
        }, cancellationToken);

        product.SetPrice(55);

        food.AddProduct(product);


        var patty = new ChoiceOption("Patty");

        product.AddOption(patty);

        await context.SaveChangesAsync();

        var pattyBeef = new OptionValue("Beef");

        patty.AddValue(pattyBeef);

        var pattyVeg = new OptionValue("Veg");

        patty.DefaultValue = pattyBeef;

        patty.AddValue(pattyVeg);


        var optionLettuce = new SelectableOption("Lettuce");;

        product.AddOption(optionLettuce);

        var optionTomato = new SelectableOption("Tomato"); ;

        product.AddOption(optionTomato);

        var optionCheddarCheese = new SelectableOption("Cheddar cheese"); ;

        product.AddOption(optionCheddarCheese);

        var optionOnion = new SelectableOption("Onion"); ;

        product.AddOption(optionOnion);

        var optionMustard = new SelectableOption("Mustard"); ;

        product.AddOption(optionMustard);

        var optionPickles = new SelectableOption("Pickles"); ;

        product.AddOption(optionPickles);

        var optionJalapeno = new SelectableOption("Jalapeño"); ;

        product.AddOption(optionJalapeno);


        var bread = new ChoiceOption("Bread");

        product.AddOption(bread);

        var breadBun = new OptionValue("Hamburger bun");

        bread.AddValue(breadBun);

        await context.SaveChangesAsync();

        var firscoBun = new OptionValue("Frisco bun");

        bread.DefaultValue = breadBun;

        bread.AddValue(firscoBun);

        await context.SaveChangesAsync();
    }

    public static async Task CreateCocaCola(CatalogContext context, ProductFactory productFactory, CancellationToken cancellationToken = default)
    {
        var product = await productFactory.CreateProductAsync(TenantConstants.OrganizationId, new()
        {
            Name = "Coca Cola 33cl",
            Handle = "coca-cola-33cl",
            Description = "",
            Price = 12,
            VatRate = 0.12,
            StoreId = store.Id,
            ImageId = PlaceholderImage.Id,
        }, cancellationToken);

        drinks.AddProduct(product);

        await context.SaveChangesAsync();
    }

    public static async Task CreateCocaColaZero(CatalogContext context, ProductFactory productFactory, CancellationToken cancellationToken = default)
    {
        var product = await productFactory.CreateProductAsync(TenantConstants.OrganizationId, new()
        {
            Name = "Coca Cola Zero Sugar 33cl",
            Handle = "coca-cola-zero-sugar-33cl",
            Description = "",
            Price = 12,
            VatRate = 0.12,
            StoreId = store.Id,
            ImageId = PlaceholderImage.Id,
        }, cancellationToken);

        drinks.AddProduct(product);

        await context.SaveChangesAsync();
    }

    public static async Task CreateCarlsberg(CatalogContext context, ProductFactory productFactory, CancellationToken cancellationToken = default)
    {
        var product = await productFactory.CreateProductAsync(TenantConstants.OrganizationId, new()
        {
            Name = "Carlsberg Export 33cl",
            Handle = "carlsberg-export-33cl",
            Headline = "Probably the best beer in the world!",
            Description = "Pilsner beer",
            Price = 21,
            VatRate = 0.12,
            StoreId = store.Id,
            ImageId = PlaceholderImage.Id,
        }, cancellationToken);

        drinks.AddProduct(product);

        await context.SaveChangesAsync();
    }

    public static async Task CreateHouseCleaningService(CatalogContext context, ProductFactory productFactory, CancellationToken cancellationToken = default)
    {
        var product = await productFactory.CreateProductAsync(TenantConstants.OrganizationId, new()
        {
            Name = "House cleaning",
            Handle = "house-cleaning",
            Type = ProductType.Service,
            Description = "",
            Headline = "Cleaning your house",
            ListingState = Domain.Enums.ProductListingState.Listed,
            BrandId = brand.Id,
            StoreId = store.Id,
            ImageId = PlaceholderImage.Id
        }, cancellationToken);

        var subscriptionPlan1 = new ProductSubscriptionPlan("Weekly cleaning", "Coming to your house weekly", TimeInterval.Weekly, TimeInterval.Monthly, RenewalOption.Manual, RenewalInterval.Months, 3, TimeSpan.FromDays(14), discountPercentage: 0.1);
        product.AddSubscriptionPlan(subscriptionPlan1);

        var subscriptionPlan2 = new ProductSubscriptionPlan("Monthly cleaning", "Coming to your house monthly", TimeInterval.Monthly, TimeInterval.Monthly, RenewalOption.Manual, RenewalInterval.Years, 1, TimeSpan.FromDays(30), discountPercentage: 0.3);
        product.AddSubscriptionPlan(subscriptionPlan2);

        services.AddProduct(product);

        await context.SaveChangesAsync();
    }
}
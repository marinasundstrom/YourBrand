using YourBrand.Products.Domain.Entities;
using YourBrand.Products.Domain.Enums;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Products.Infrastructure.Persistence;

public class SeedData
{
    public static async Task EnsureSeedData(WebApplication app)
    {
        using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<SeedData>>();

            var context = scope.ServiceProvider.GetRequiredService<ProductsContext>();
            await context.Database.EnsureDeletedAsync();
            //context.Database.Migrate();
            await context.Database.EnsureCreatedAsync();

            context.ProductGroups.Add(new ProductGroup()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Products",
                Description = null
            });

            context.ProductGroups.Add(new ProductGroup()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Clothes",
                Description = null
            });

            context.ProductGroups.Add(new ProductGroup()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Food",
                Description = null
            });

            await context.SaveChangesAsync();

            await CreateShirt2(context);

            await CreateKebabPlate(context);

            await CreateHerrgardsStek(context);

            await CreatePizza(context);

            await CreateSalad(context);

            await context.SaveChangesAsync();
        }
    }

    public static async Task CreateShirt(ProductsContext context)
    {
        var product = new Product()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Randing t-shirt",
            Description = "Stilren t-shirt med randigt mönster",
            HasVariants = true,
            Group = await context.ProductGroups.FirstAsync(x => x.Name == "Clothes")
        };

        context.Products.Add(product);

        var option = new Domain.Entities.Attribute()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Size",
            ForVariant = true
        };

        product.Attributes.Add(option);

        var valueSmall = new AttributeValue
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Small"
        };

        option.Values.Add(valueSmall);

        var valueMedium = new AttributeValue
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Medium"
        };

        option.Values.Add(valueMedium);

        var valueLarge = new AttributeValue
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Large"
        };

        option.Values.Add(valueLarge);

        product.Attributes.Add(option);

        var variantSmall = new ProductVariant()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Small",
            SKU = "12345667",
            UPC = "4345547457457",
        };

        variantSmall.Values.Add(new VariantValue()
        {
            Attribute = option,
            Value = valueSmall
        });

        product.Variants.Add(variantSmall);

        var variantMedium = new ProductVariant()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Medium",
            SKU = "4465645645",
            UPC = "543453454567",
        };

        variantMedium.Values.Add(new VariantValue()
        {
            Attribute = option,
            Value = valueMedium
        });

        product.Variants.Add(variantMedium);

        var variantLarge = new ProductVariant()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Large",
            SKU = "233423544545",
            UPC = "6876345345345",
        };

        variantLarge.Values.Add(new VariantValue()
        {
            Attribute = option,
            Value = valueLarge
        });

        product.Variants.Add(variantLarge);
    }

    public static async Task CreateShirt2(ProductsContext context)
    {
        var product = new Product()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "T-shirt",
            Description = "T-shirt i olika färger",
            HasVariants = true,
            Group = await context.ProductGroups.FirstAsync(x => x.Name == "Clothes"),
            Visibility = ProductVisibility.Listed
        };

        context.Products.Add(product);

        var option = new Domain.Entities.Attribute()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Size",
            ForVariant = true
        };

        product.Attributes.Add(option);

        var valueSmall = new AttributeValue
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Small"
        };

        option.Values.Add(valueSmall);

        var valueMedium = new AttributeValue
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Medium"
        };

        option.Values.Add(valueMedium);

        var valueLarge = new AttributeValue
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Large"
        };

        option.Values.Add(valueLarge);

        product.Attributes.Add(option);

        var option2 = new Domain.Entities.Attribute()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Color",
            ForVariant = true
        };

        product.Attributes.Add(option2);

        var valueBlue = new AttributeValue
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Blue"
        };

        option2.Values.Add(valueBlue);

        var valueRed = new AttributeValue
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Red"
        };

        option2.Values.Add(valueRed);

        ///*

        var variantBlueSmall = new ProductVariant()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Blue S",
            SKU = "TSHIRT-BLUE-S",
            UPC = "4345547457457",
            Price = 120,
        };

        variantBlueSmall.Values.Add(new VariantValue()
        {
            Attribute = option,
            Value = valueSmall
        });

        variantBlueSmall.Values.Add(new VariantValue()
        {
            Attribute = option2,
            Value = valueBlue
        });

        product.Variants.Add(variantBlueSmall);

        //*/

        var variantBlueMedium = new ProductVariant()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Blue M",
            SKU = "TSHIRT-BLUE-M",
            UPC = "543453454567",
            Price = 120
        };

        variantBlueMedium.Values.Add(new VariantValue()
        {
            Attribute = option,
            Value = valueMedium
        });

        variantBlueMedium.Values.Add(new VariantValue()
        {
            Attribute = option2,
            Value = valueBlue
        });

        product.Variants.Add(variantBlueMedium);

        var variantBlueLarge = new ProductVariant()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Blue L",
            SKU = "TSHIRT-BLUE-L",
            UPC = "6876345345345",
            Price = 60,
        };

        variantBlueLarge.Values.Add(new VariantValue()
        {
            Attribute = option,
            Value = valueLarge
        });

        variantBlueLarge.Values.Add(new VariantValue()
        {
            Attribute = option2,
            Value = valueBlue
        });

        product.Variants.Add(variantBlueLarge);

        /////

        var variantRedSmall = new ProductVariant()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Red S",
            SKU = "TSHIRT-RED-S",
            UPC = "4345547457457",
            Price = 120,
        };

        variantRedSmall.Values.Add(new VariantValue()
        {
            Attribute = option,
            Value = valueSmall
        });

        variantRedSmall.Values.Add(new VariantValue()
        {
            Attribute = option2,
            Value = valueRed
        });

        product.Variants.Add(variantRedSmall);

        var variantRedMedium = new ProductVariant()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Red M",
            SKU = "TSHIRT-RED-M",
            UPC = "543453454567",
            Price = 120,
        };

        variantRedMedium.Values.Add(new VariantValue()
        {
            Attribute = option,
            Value = valueMedium
        });

        variantRedMedium.Values.Add(new VariantValue()
        {
            Attribute = option2,
            Value = valueRed
        });

        product.Variants.Add(variantRedMedium);

        var variantRedLarge = new ProductVariant()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Red L",
            SKU = "TSHIRT-RED-L",
            UPC = "6876345345345",
            Price = 120,
        };

        variantRedLarge.Values.Add(new VariantValue()
        {
            Attribute = option,
            Value = valueLarge
        });

        variantRedLarge.Values.Add(new VariantValue()
        {
            Attribute = option2,
            Value = valueRed
        });

        product.Variants.Add(variantRedLarge);
    }

    public static async Task CreateKebabPlate(ProductsContext context)
    {
        var product = new Product()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Kebabtallrik",
            Description = "Dönnerkebab, nyfriterad pommes frites, sallad, och sås",
            Price = 89,
            Group = await context.ProductGroups.FirstAsync(x => x.Name == "Food")
        };

        context.Products.Add(product);

        var option = new Option()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Sås"
        };

        product.Options.Add(option);

        await context.SaveChangesAsync();

        var valueSmall = new OptionValue
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Mild"
        };

        option.Values.Add(valueSmall);

        var valueMedium = new OptionValue
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Stark"
        };

        option.Values.Add(valueMedium);

        var valueLarge = new OptionValue
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Blandad"
        };

        option.DefaultValue = valueSmall;

        option.Values.Add(valueLarge);
    }

    public static async Task CreateHerrgardsStek(ProductsContext context)
    {
        var product = new Product()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Herrgårdsstek",
            Description = "Vår fina stek med pommes och vår hemlagade bearnaise sås",
            Price = 179,
            Group = await context.ProductGroups.FirstAsync(x => x.Name == "Food")
        };

        context.Products.Add(product);

        await context.SaveChangesAsync();

        var optionDoneness = new Option()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Stekning"
        };

        product.Options.Add(optionDoneness);

        await context.SaveChangesAsync();

        optionDoneness.Values.Add(new OptionValue()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Rare",
            Seq = 1
        });

        var optionMediumRare = new OptionValue()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Medium Rare",
            Seq = 2
        };

        optionDoneness.Values.Add(optionMediumRare);

        optionDoneness.Values.Add(new OptionValue()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Well Done",
            Seq = 3
        });

        optionDoneness.DefaultValue = optionMediumRare;

        var optionSize = new Option()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Extra stor - 50 g mer",
            Price = 15
        };

        product.Options.Add(optionSize);

        await context.SaveChangesAsync();
    }

    public static async Task CreatePizza(ProductsContext context)
    {
        var product = new Product()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Pizza",
            Description = "Custom pizza",
            Price = 40,
            Group = await context.ProductGroups.FirstAsync(x => x.Name == "Food")
        };

        context.Products.Add(product);

        await context.SaveChangesAsync();

        var breadGroup = new OptionGroup()
        {
            Id = Guid.NewGuid().ToString(),
            Seq = 1,
            Name = "Bread"
        };

        product.OptionGroups.Add(breadGroup);

        var meatGroup = new OptionGroup()
        {
            Id = Guid.NewGuid().ToString(),
            Seq = 2,
            Name = "Meat",
            Max = 2
        };

        product.OptionGroups.Add(meatGroup);

        var nonMeatGroup = new OptionGroup()
        {
            Id = Guid.NewGuid().ToString(),
            Seq = 3,
            Name = "Non-Meat"
        };

        product.OptionGroups.Add(nonMeatGroup);

        var sauceGroup = new OptionGroup()
        {
            Id = Guid.NewGuid().ToString(),
            Seq = 4,
            Name = "Sauce"
        };

        product.OptionGroups.Add(sauceGroup);

        var toppingsGroup = new OptionGroup()
        {
            Id = Guid.NewGuid().ToString(),
            Seq = 5,
            Name = "Toppings"
        };

        product.OptionGroups.Add(toppingsGroup);

        await context.SaveChangesAsync();

        var optionStyle = new Option()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Style"
        };

        product.Options.Add(optionStyle);

        await context.SaveChangesAsync();

        var valueItalian = new OptionValue
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Italian"
        };

        optionStyle.Values.Add(valueItalian);

        var valueAmerican = new OptionValue
        {
            Id = Guid.NewGuid().ToString(),
            Name = "American"
        };

        optionStyle.DefaultValue = valueAmerican;

        optionStyle.Values.Add(valueAmerican);

        var optionHam = new Option()
        {
            Id = Guid.NewGuid().ToString(),
            OptionType = OptionType.Single,
            Name = "Ham",
            Group = meatGroup,
            Price = 15
        };

        product.Options.Add(optionHam);

        var optionKebab = new Option()
        {
            Id = Guid.NewGuid().ToString(),
            OptionType = OptionType.Single,
            Name = "Kebab",
            Group = meatGroup,
            Price = 10,
            IsSelected = true
        };

        product.Options.Add(optionKebab);

        var optionChicken = new Option()
        {
            Id = Guid.NewGuid().ToString(),
            OptionType = OptionType.Single,
            Name = "Chicken",
            Group = meatGroup,
            Price = 10
        };

        product.Options.Add(optionChicken);

        var optionExtraCheese = new Option()
        {
            Id = Guid.NewGuid().ToString(),
            OptionType = OptionType.Single,
            Name = "Extra cheese",
            Group = toppingsGroup,
            Price = 5
        };

        product.Options.Add(optionExtraCheese);

        var optionGreenOlives = new Option()
        {
            Id = Guid.NewGuid().ToString(),
            OptionType = OptionType.Single,
            Name = "Green Olives",
            Group = toppingsGroup,
            Price = 5
        };

        product.Options.Add(optionGreenOlives);
    }

    public static async Task CreateSalad(ProductsContext context)
    {
        var product = new Product()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Sallad",
            Description = "Din egna sallad",
            Price = 52,
            Group = await context.ProductGroups.FirstAsync(x => x.Name == "Food"),
            Visibility = ProductVisibility.Listed
        };

        context.Products.Add(product);

        var baseGroup = new OptionGroup()
        {
            Id = Guid.NewGuid().ToString(),
            Seq = 1,
            Name = "Bas"
        };

        product.OptionGroups.Add(baseGroup);

        var proteinGroup = new OptionGroup()
        {
            Id = Guid.NewGuid().ToString(),
            Seq = 2,
            Name = "Välj protein",
            Max = 1
        };

        product.OptionGroups.Add(proteinGroup);

        var additionalGroup = new OptionGroup()
        {
            Id = Guid.NewGuid().ToString(),
            Seq = 4,
            Name = "Välj tillbehör",
            Max = 3
        };

        product.OptionGroups.Add(additionalGroup);

        var dressingGroup = new OptionGroup()
        {
            Id = Guid.NewGuid().ToString(),
            Seq = 5,
            Name = "Välj dressing",
            Max = 1
        };

        product.OptionGroups.Add(dressingGroup);

        await context.SaveChangesAsync();

        var optionBase = new Option()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Bas",
            Group = baseGroup
        };

        product.Options.Add(optionBase);

        await context.SaveChangesAsync();

        var valueSallad = new OptionValue
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Sallad",
        };

        optionBase.Values.Add(valueSallad);

        var valueSalladPasta = new OptionValue
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Sallad med pasta"
        };

        optionBase.DefaultValue = valueSalladPasta;

        optionBase.Values.Add(valueSalladPasta);

        var valueSalladQuinoa = new OptionValue
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Sallad med quinoa",
        };

        optionBase.Values.Add(valueSalladQuinoa);

        var valueSalladNudlar = new OptionValue
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Sallad med glasnudlar",
        };

        optionBase.Values.Add(valueSalladNudlar);

        var optionChicken = new Option()
        {
            Id = Guid.NewGuid().ToString(),
            OptionType = OptionType.Single,
            Name = "Kycklingfilé",
            Group = proteinGroup
        };

        product.Options.Add(optionChicken);

        var optionSmokedTurkey = new Option()
        {
            Id = Guid.NewGuid().ToString(),
            OptionType = OptionType.Single,
            Name = "Rökt kalkonfilé",
            Group = proteinGroup
        };

        product.Options.Add(optionSmokedTurkey);

        var optionBeanMix = new Option()
        {
            Id = Guid.NewGuid().ToString(),
            OptionType = OptionType.Single,
            Name = "Marinerad bönmix",
            Group = proteinGroup
        };

        product.Options.Add(optionBeanMix);

        var optionVegMe = new Option()
        {
            Id = Guid.NewGuid().ToString(),
            OptionType = OptionType.Single,
            Name = "VegMe",
            Group = proteinGroup
        };

        product.Options.Add(optionVegMe);

        var optionChevre = new Option()
        {
            Id = Guid.NewGuid().ToString(),
            OptionType = OptionType.Single,
            Name = "Chevré",
            Group = proteinGroup
        };

        product.Options.Add(optionChevre);

        var optionSmokedSalmon = new Option()
        {
            Id = Guid.NewGuid().ToString(),
            OptionType = OptionType.Single,
            Name = "Varmrökt lax",
            Group = proteinGroup
        };

        product.Options.Add(optionSmokedSalmon);

        var optionPrawns = new Option()
        {
            Id = Guid.NewGuid().ToString(),
            OptionType = OptionType.Single,
            Name = "Handskalade räkor",
            Group = proteinGroup
        };

        product.Options.Add(optionPrawns);

        var optionCheese = new Option()
        {
            Id = Guid.NewGuid().ToString(),
            OptionType = OptionType.Single,
            Name = "Parmesanost",
            Group = additionalGroup
        };

        product.Options.Add(optionCheese);

        var optionGreenOlives = new Option()
        {
            Id = Guid.NewGuid().ToString(),
            OptionType = OptionType.Single,
            Name = "Gröna oliver",
            Group = additionalGroup
        };

        product.Options.Add(optionGreenOlives);

        var optionSoltorkadTomat = new Option()
        {
            Id = Guid.NewGuid().ToString(),
            OptionType = OptionType.Single,
            Name = "Soltorkade tomater",
            Group = additionalGroup
        };

        product.Options.Add(optionSoltorkadTomat);

        var optionInlagdRödlök = new Option()
        {
            Id = Guid.NewGuid().ToString(),
            OptionType = OptionType.Single,
            Name = "Inlagd rödlök",
            Group = additionalGroup
        };

        product.Options.Add(optionInlagdRödlök);

        var optionRostadAioli = new Option()
        {
            Id = Guid.NewGuid().ToString(),
            OptionType = OptionType.Single,
            Name = "Rostad aioli",
            Group = dressingGroup
        };

        product.Options.Add(optionRostadAioli);

        var optionPesto = new Option()
        {
            Id = Guid.NewGuid().ToString(),
            OptionType = OptionType.Single,
            Name = "Pesto",
            Group = dressingGroup
        };

        product.Options.Add(optionPesto);

        var optionOrtvinagret = new Option()
        {
            Id = Guid.NewGuid().ToString(),
            OptionType = OptionType.Single,
            Name = "Örtvinägrett",
            Group = dressingGroup
        };

        product.Options.Add(optionOrtvinagret);

        var optionSoyavinagret = new Option()
        {
            Id = Guid.NewGuid().ToString(),
            OptionType = OptionType.Single,
            Name = "Soyavinägrett",
            Group = dressingGroup
        };

        product.Options.Add(optionSoyavinagret);

        var optionRhodeIsland = new Option()
        {
            Id = Guid.NewGuid().ToString(),
            OptionType = OptionType.Single,
            Name = "Rhode Island",
            Group = dressingGroup
        };

        product.Options.Add(optionRhodeIsland);

        var optionKimchimayo = new Option()
        {
            Id = Guid.NewGuid().ToString(),
            OptionType = OptionType.Single,
            Name = "Kimchimayo",
            Group = dressingGroup
        };

        product.Options.Add(optionKimchimayo);

        var optionCaesar = new Option()
        {
            Id = Guid.NewGuid().ToString(),
            OptionType = OptionType.Single,
            Name = "Caesar",
            Group = dressingGroup
        };

        product.Options.Add(optionCaesar);

        var optionCitronLime = new Option()
        {
            Id = Guid.NewGuid().ToString(),
            OptionType = OptionType.Single,
            Name = "Citronlime",
            Group = dressingGroup
        };

        product.Options.Add(optionCitronLime);
    }
}
namespace BlazorApp.Brands;

public static class Mapper
{
    public static Brand Map(this YourBrand.StoreFront.Brand brand)
        => new(brand.Id, brand.Name, null);
}

public record Brand(long Id, string Name, Brand? Parent);
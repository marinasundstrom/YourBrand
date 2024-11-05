namespace YourBrand.Portal.Branding;

public static class BrandProfileServiceExtensions
{
    public static async Task LoadBrandProfileAsync(this IServiceProvider serviceProvider)
    {
        var brandProfileService = serviceProvider.GetRequiredService<BrandProfileService>();
        await brandProfileService.LoadBrandProfileAsync();
    }
}
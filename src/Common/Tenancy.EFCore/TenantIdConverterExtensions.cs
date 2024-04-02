using Microsoft.EntityFrameworkCore;

namespace YourBrand.Tenancy;

public static class TenantIdConverterExtensions
{
    public static ModelConfigurationBuilder AddTenantIdConverter(this ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<TenantId>().HaveConversion<TenantIdConverter>();

        return configurationBuilder;
    }
}
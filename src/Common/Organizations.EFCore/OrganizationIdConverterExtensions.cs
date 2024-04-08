using Microsoft.EntityFrameworkCore;

namespace YourBrand.Domain;

public static class OrganizationIdConverterExtensions
{
    public static ModelConfigurationBuilder AddOrganizationIdConverter(this ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<OrganizationId>().HaveConversion<OrganizationIdConverter>();

        return configurationBuilder;
    }
}
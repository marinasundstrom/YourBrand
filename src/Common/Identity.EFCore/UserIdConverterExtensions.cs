using Microsoft.EntityFrameworkCore;

namespace YourBrand.Identity;

public static class UserIdConverterExtensions
{
    public static ModelConfigurationBuilder AddUserIdConverter(this ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<UserId>().HaveConversion<UserIdConverter>();

        return configurationBuilder;
    }
}
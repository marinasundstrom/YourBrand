
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand.IdentityManagement.Infrastructure.Persistence.Configurations;

public class IdentityUserLoginConfiguration : IEntityTypeConfiguration<IdentityUserLogin<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserLogin<string>> builder)
    {
        builder.ToTable("UserLogins");
        //in case you chagned the TKey type
        //  builder.HasKey(key => new { key.ProviderKey, key.LoginProvider });  
    }
}
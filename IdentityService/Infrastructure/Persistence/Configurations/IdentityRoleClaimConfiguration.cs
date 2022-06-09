
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand.IdentityService.Infrastructure.Persistence.Configurations;

public class IdentityRoleClaimConfiguration : IEntityTypeConfiguration<IdentityRoleClaim<string>>
{
    public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>> builder)
    {
        builder.ToTable("RoleClaims"); 
    }
}

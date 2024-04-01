using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.IdentityManagement.Domain.Entities;

namespace YourBrand.IdentityManagement.Infrastructure.Persistence.Configurations;

public class OrganizationUserConfiguration : IEntityTypeConfiguration<OrganizationUser>
{
    public void Configure(EntityTypeBuilder<OrganizationUser> builder)
    {
        builder.ToTable(name: "OrganizationUsers");
    }
}

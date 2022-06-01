using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.IdentityService.Domain.Entities;

namespace YourBrand.IdentityService.Infrastructure.Persistence.Configurations;

public class UserDependantConfiguration : IEntityTypeConfiguration<UserDependant>
{
    public void Configure(EntityTypeBuilder<UserDependant> builder)
    {
        builder.ToTable(name: "UserDependants");
    }
}

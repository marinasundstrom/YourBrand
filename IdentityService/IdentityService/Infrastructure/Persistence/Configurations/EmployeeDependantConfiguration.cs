using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.IdentityService.Domain.Entities;

namespace YourBrand.IdentityService.Infrastructure.Persistence.Configurations;

public class EmployeeDependantConfiguration : IEntityTypeConfiguration<EmployeeDependant>
{
    public void Configure(EntityTypeBuilder<EmployeeDependant> builder)
    {
        builder.ToTable(name: "EmployeeDependants");
    }
}

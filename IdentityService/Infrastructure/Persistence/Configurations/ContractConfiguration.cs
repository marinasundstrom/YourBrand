using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.IdentityService.Domain.Entities;

namespace YourBrand.IdentityService.Infrastructure.Persistence.Configurations;

public class ContractConfiguration : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.ToTable(name: "Contracts");

        builder.OwnsOne(x => x.Salary);
    }
}

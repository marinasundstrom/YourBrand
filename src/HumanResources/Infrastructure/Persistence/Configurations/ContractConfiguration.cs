using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.HumanResources.Domain.Entities;

namespace YourBrand.HumanResources.Infrastructure.Persistence.Configurations;

public class ContractConfiguration : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.ToTable(name: "Contracts");

        builder.OwnsOne(x => x.Salary);
    }
}
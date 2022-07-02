using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.HumanResources.Domain.Entities;

namespace YourBrand.HumanResources.Infrastructure.Persistence.Configurations;

public class PersonDependantConfiguration : IEntityTypeConfiguration<PersonDependant>
{
    public void Configure(EntityTypeBuilder<PersonDependant> builder)
    {
        builder.ToTable(name: "PersonDependants");
    }
}

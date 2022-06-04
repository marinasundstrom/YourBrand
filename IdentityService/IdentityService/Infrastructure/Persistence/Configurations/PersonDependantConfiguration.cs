using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.IdentityService.Domain.Entities;

namespace YourBrand.IdentityService.Infrastructure.Persistence.Configurations;

public class PersonDependantConfiguration : IEntityTypeConfiguration<PersonDependant>
{
    public void Configure(EntityTypeBuilder<PersonDependant> builder)
    {
        builder.ToTable(name: "PersonDependants");
    }
}

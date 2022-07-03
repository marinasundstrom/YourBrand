using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.HumanResources.Domain.Entities;

namespace YourBrand.HumanResources.Infrastructure.Persistence.Configurations;

public class PersonRoleConfiguration : IEntityTypeConfiguration<PersonRole>
{
    public void Configure(EntityTypeBuilder<PersonRole> builder)
    {
        builder.ToTable("PersonRoles");

        //in case you chagned the TKey type
        //  builder.HasKey(key => new { key.PersonId, key.RoleId });

        builder.Property(e => e.PersonId).HasColumnName("PersonId");
    }
}

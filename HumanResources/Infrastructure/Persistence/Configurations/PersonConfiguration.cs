
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.HumanResources.Domain.Entities;

namespace YourBrand.HumanResources.Infrastructure.Persistence.Configurations;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable(name: "Persons");

        builder.HasQueryFilter(i => i.Deleted == null);

        builder.HasMany(u => u.Roles)
            .WithMany(r => r.Persons)
            .UsingEntity<PersonRole>(
                j => j
                    .HasOne(pt => pt.Role)
                    .WithMany(p => p.PersonRoles)
                    .HasForeignKey(pt => pt.RoleId),
                j => j
                    .HasOne(pt => pt.Person)
                    .WithMany(t => t.PersonRoles)
                    .HasForeignKey(pt => pt.PersonId));
    }
}

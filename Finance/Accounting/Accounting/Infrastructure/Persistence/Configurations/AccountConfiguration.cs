using System;

using YourBrand.Accounting.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand.Accounting.Infrastructure.Persistence.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");
        //builder.HasQueryFilter(i => i.Deleted == null);

        builder.HasKey(x => x.AccountNo);

        builder
            .Property(x => x.AccountNo)
            .ValueGeneratedNever();
    }
}
using System;

using Accounting.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accounting.Infrastructure.Persistence.Configurations;

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
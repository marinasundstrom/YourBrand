using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Accounting.Domain.Entities;

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

        builder.HasIndex(x => x.TenantId);
    }
}
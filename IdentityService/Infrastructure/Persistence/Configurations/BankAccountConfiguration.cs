using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.IdentityService.Domain.Entities;

namespace YourBrand.IdentityService.Infrastructure.Persistence.Configurations;

public class BankAccountConfiguration : IEntityTypeConfiguration<BankAccount>
{
    public void Configure(EntityTypeBuilder<BankAccount> builder)
    {
        builder.ToTable(name: "BankAccounts");

        builder
            .HasOne(p => p.Person)
            .WithOne()
            .HasForeignKey(nameof(BankAccount), "UserId");
    }
}

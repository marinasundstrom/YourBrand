
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Infrastructure.Persistence.Configurations;

public class TaskConfiguration : IEntityTypeConfiguration<Domain.Entities.Task>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Task> builder)
    {
        builder.ToTable("Tasks");

        builder.HasMany(x => x.Charges)
            .WithOne(x => x.Task);

        builder.Ignore(x => x.DomainEvents);

        builder.HasOne(x => x.CreatedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.LastModifiedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.DeletedBy)
            .WithMany()
            .OnDelete(DeleteBehavior.NoAction);
    }
}

public class TaskChargeConfiguration : IEntityTypeConfiguration<Domain.Entities.TaskCharge>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.TaskCharge> builder)
    {
        builder.ToTable("TaskCharges");

        builder.Property(x => x.Type)
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.Property(x => x.Basis)
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.Property(x => x.Amount)
            .HasPrecision(18, 4);
    }
}
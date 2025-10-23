using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Meetings.Domain.Entities;

namespace YourBrand.Agendas.Infrastructure.Persistence.Configurations;

public sealed class AgendaItemTypeConfiguration : IEntityTypeConfiguration<AgendaItemType>
{
    public void Configure(EntityTypeBuilder<AgendaItemType> builder)
    {
        builder.ToTable("AgendaItemTypes");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100); // Adjust length as needed
            
        builder.Property(e => e.Description)
            .HasMaxLength(250); // Adjust length as needed

        builder.HasOne(x => x.HandledByFunction)
            .WithMany()
            .HasForeignKey(x => x.HandledByFunctionId)
            .OnDelete(DeleteBehavior.NoAction);

    }
}

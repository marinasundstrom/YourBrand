using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand.Agendas.Infrastructure.Persistence.Configurations;

public sealed class MeetingFunctionConfiguration : IEntityTypeConfiguration<MeetingFunction>
{
    public void Configure(EntityTypeBuilder<MeetingFunction> builder)
    {
        builder.ToTable("MeetingFunctions");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100); // Adjust length as needed

        builder.Property(e => e.Description)
            .HasMaxLength(250); // Adjust length as needed
    }
}
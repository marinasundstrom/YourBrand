using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand.Agendas.Infrastructure.Persistence.Configurations;

public sealed class MemberRoleConfiguration : IEntityTypeConfiguration<MemberRole>
{
    public void Configure(EntityTypeBuilder<MemberRole> builder)
    {
        builder.ToTable("MemberRoles");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100); // Adjust length as needed
            
        builder.Property(e => e.Description)
            .HasMaxLength(250); // Adjust length as needed
    }
}

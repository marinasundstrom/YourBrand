
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Showroom.Domain.Entities;

namespace YourBrand.Showroom.Infrastructure.Persistence.Configurations;

sealed class ProfileProjectConfiguration : IEntityTypeConfiguration<PersonProfileProject>
{
    public void Configure(EntityTypeBuilder<PersonProfileProject> builder)
    {
        builder.ToTable("PersonProfileProjects");

        builder.HasIndex(x => x.TenantId);
    }
}

sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");

        builder.HasIndex(x => x.TenantId);
    }
}


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Skynet.TimeReport.Domain.Entities;

namespace Skynet.TimeReport.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", t => t.IsTemporal());
        builder.HasQueryFilter(i => i.Deleted == null);
    }
}
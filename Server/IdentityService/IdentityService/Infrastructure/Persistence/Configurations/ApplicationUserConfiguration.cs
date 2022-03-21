
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Skynet.IdentityService.Domain.Entities;

namespace Skynet.IdentityService.Infrastructure.Persistence.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasQueryFilter(i => i.Deleted == null);

        builder.HasMany(u => u.Roles)
            .WithMany(r => r.Users)
            .UsingEntity<UserRole>(
                j => j
                    .HasOne(pt => pt.Role)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(pt => pt.RoleId),
                j => j
                    .HasOne(pt => pt.User)
                    .WithMany(t => t.UserRoles)
                    .HasForeignKey(pt => pt.UserId));
    }
}
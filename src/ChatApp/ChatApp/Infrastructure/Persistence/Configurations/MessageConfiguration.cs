using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand.ChatApp.Infrastructure.Persistence.Configurations;

public sealed class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("Messages");

        builder.HasKey(x => new { x.OrganizationId, x.Id });

        builder.OwnsMany(x => x.Reactions, x => x.ToJson());

        builder.HasOne<Channel>().WithMany().HasForeignKey(nameof(Message.OrganizationId), nameof(Message.ChannelId));

        builder.HasOne(x => x.PostedBy).WithMany().HasForeignKey(x => new { x.OrganizationId, x.PostedById });
        builder.HasOne(x => x.LastEditedBy).WithMany().HasForeignKey(x => new { x.OrganizationId, x.LastEditedById });
        builder.HasOne(x => x.DeletedBy).WithMany().HasForeignKey(x => new { x.OrganizationId, x.DeletedById });

        builder.Navigation(x => x.PostedBy).AutoInclude();
        builder.Navigation(x => x.LastEditedBy).AutoInclude();
        builder.Navigation(x => x.DeletedBy).AutoInclude();
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand.ChatApp.Infrastructure.Persistence.Configurations;

public sealed class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("Messages");

        builder.OwnsMany(x => x.Reactions, x => x.ToJson());

        builder.Navigation(x => x.PostedBy).AutoInclude();
        builder.Navigation(x => x.LastEditedBy).AutoInclude();
        builder.Navigation(x => x.DeletedBy).AutoInclude();
    }
}
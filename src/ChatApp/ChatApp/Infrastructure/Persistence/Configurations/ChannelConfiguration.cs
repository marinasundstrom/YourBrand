using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand.ChatApp.Infrastructure.Persistence.Configurations;

public sealed class ChannelConfiguration : IEntityTypeConfiguration<Channel>
{
    public void Configure(EntityTypeBuilder<Channel> builder)
    {
        builder.ToTable("Channels");

        builder.OwnsOne(x => x.Settings);

        //builder.OwnsMany(x => x.Participants, x => x.ToTable("ChannelParticipants"));
    }
}

public sealed class ChannelParticipantConfiguration : IEntityTypeConfiguration<ChannelParticipant>
{
    public void Configure(EntityTypeBuilder<ChannelParticipant> builder)
    {
        builder.ToTable("ChannelParticipants");
    }
}
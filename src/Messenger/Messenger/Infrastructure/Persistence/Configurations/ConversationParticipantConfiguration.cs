
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Messenger.Domain.Entities;

namespace YourBrand.Messenger.Infrastructure.Persistence.Configurations;

sealed class ConversationParticipantConfiguration : IEntityTypeConfiguration<ConversationParticipant>
{
    public void Configure(EntityTypeBuilder<ConversationParticipant> builder)
    {
        builder.ToTable("ConversationParticipants");
        builder.HasQueryFilter(i => i.Deleted == null);

        builder.Ignore(i => i.DomainEvents);
    }
}
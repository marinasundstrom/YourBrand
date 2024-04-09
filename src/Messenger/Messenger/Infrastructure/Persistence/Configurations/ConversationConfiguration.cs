
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Messenger.Domain.Entities;

namespace YourBrand.Messenger.Infrastructure.Persistence.Configurations;

sealed class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.ToTable("Conversations");
        builder.HasQueryFilter(i => i.Deleted == null);

        builder.Ignore(i => i.DomainEvents);
    }
}
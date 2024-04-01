
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Messenger.Domain.Entities;

namespace YourBrand.Messenger.Infrastructure.Persistence.Configurations;

class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("Messages");
        builder.HasQueryFilter(i => i.Deleted == null);

        builder.Ignore(i => i.DomainEvents);
    }
}
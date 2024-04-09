
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Messenger.Domain.Entities;

namespace YourBrand.Messenger.Infrastructure.Persistence.Configurations;

sealed class MessageReceiptConfiguration : IEntityTypeConfiguration<MessageReceipt>
{
    public void Configure(EntityTypeBuilder<MessageReceipt> builder)
    {
        builder.ToTable("MessageReceipts");

        builder.Ignore(i => i.DomainEvents);
    }
}
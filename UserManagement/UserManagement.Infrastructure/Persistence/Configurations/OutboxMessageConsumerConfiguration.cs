using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.UserManagement.Infrastructure.Persistence.Outbox;

namespace YourBrand.UserManagement.Infrastructure.Persistence.Configurations;

public sealed class OutboxMessageConsumerConfiguration : IEntityTypeConfiguration<OutboxMessageConsumer>
{
    public void Configure(EntityTypeBuilder<OutboxMessageConsumer> builder)
    {
        builder.ToTable("OutboxMessageConsumers");
    }
}
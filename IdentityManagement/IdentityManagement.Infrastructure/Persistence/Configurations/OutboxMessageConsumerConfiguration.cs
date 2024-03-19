using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.IdentityManagement.Infrastructure.Persistence.Outbox;

namespace YourBrand.IdentityManagement.Infrastructure.Persistence.Configurations;

public sealed class OutboxMessageConsumerConfiguration : IEntityTypeConfiguration<OutboxMessageConsumer>
{
    public void Configure(EntityTypeBuilder<OutboxMessageConsumer> builder)
    {
        builder.ToTable("OutboxMessageConsumers");
    }
}
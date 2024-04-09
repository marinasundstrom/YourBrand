using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using Polly;
using Polly.Retry;

using Quartz;

using YourBrand.Invoicing.Application.Common.Interfaces;
using YourBrand.Invoicing.Domain.Common;
using YourBrand.Invoicing.Infrastructure.Persistence;
using YourBrand.Invoicing.Infrastructure.Persistence.Outbox;

namespace YourBrand.Invoicing.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
public sealed class ProcessOutboxMessagesJob(InvoicingContext dbContext, IDomainEventDispatcher domainEventDispatcher,
    ILogger<ProcessOutboxMessagesJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogDebug("Processing Outbox");

        List<OutboxMessage> messages = await dbContext
            .Set<OutboxMessage>()
            .Where(m => m.ProcessedOnUtc == null)
            .OrderBy(m => m.OccurredOnUtc)
            .Take(20)
            .ToListAsync(context.CancellationToken);

        foreach (OutboxMessage outboxMessage in messages)
        {
            DomainEvent? domainEvent = JsonConvert
                .DeserializeObject<DomainEvent>(outboxMessage.Content, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });

            if (domainEvent is null)
            {
                continue;
            }

            AsyncRetryPolicy policy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(3, attempt => TimeSpan.FromMicroseconds(50 * attempt));

            PolicyResult result = await policy.ExecuteAndCaptureAsync(() =>
                domainEventDispatcher.Dispatch(domainEvent, context.CancellationToken));

            outboxMessage.Error = result.FinalException?.ToString();
            outboxMessage.ProcessedOnUtc = DateTime.UtcNow;
        }

        logger.LogDebug("Finished processing Outbox");

        await dbContext.SaveChangesAsync(context.CancellationToken);
    }
}
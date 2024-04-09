using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using Polly;
using Polly.Retry;

using Quartz;

using YourBrand.RotRutService.Application.Common.Interfaces;
using YourBrand.RotRutService.Domain.Common;
using YourBrand.RotRutService.Infrastructure.Persistence;
using YourBrand.RotRutService.Infrastructure.Persistence.Outbox;

namespace YourBrand.RotRutService.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
public sealed class ProcessOutboxMessagesJob(RotRutContext dbContext, IDomainEventDispatcher domainEventDispatcher,
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
using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Domain.Enums;

namespace YourBrand.Sales.Features.SubscriptionManagement;

public record SubscriptionPlanDto(Guid Id, string Name, string? ProductId, SubscriptionSchedule Schedule, bool HasTrial, TimeSpan TrialLength, TimeSpan? CancellationFinalizationPeriod, RenewalOption RenewalOption) : Domain.Entities.ISubscriptionParameters;

public record SubscriptionPlanShortDto(Guid Id, string Name, string? ProductId);
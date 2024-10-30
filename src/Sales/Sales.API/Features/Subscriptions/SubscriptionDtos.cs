using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Domain.Enums;
using YourBrand.Sales.Features.OrderManagement.Orders.Dtos;

namespace YourBrand.Sales.Features.SubscriptionManagement;

public record SubscriptionDto(Guid Id, int SubscriptionNo, SubscriptionTypeDto Type, SubscriptionPlanShortDto Plan, SubscriptionStatusDto Status, DateOnly StartDate, DateOnly? EndDate, OrderShortDto? Order, string? OrderItemId, TimeSpan? CancellationFinalizationPeriod, RenewalOption RenewalOption, SubscriptionSchedule Schedule) : Domain.Entities.ISubscriptionParameters;
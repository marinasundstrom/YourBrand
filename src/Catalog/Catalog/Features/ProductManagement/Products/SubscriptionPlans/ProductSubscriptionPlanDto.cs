namespace YourBrand.Catalog.Features.ProductManagement.SubscriptionPlans.SubscriptionPlans;

public record ProductSubscriptionPlanDto(string Id, string Title, decimal Price, ProductSubscriptionPlanTrialDto? Trial);

public record ProductSubscriptionPlanTrialDto(int PeriodLength, decimal Price);

public static class Mappings
{
    public static ProductSubscriptionPlanDto ToDto(this Domain.Entities.ProductSubscriptionPlan subscriptionPlan)
    {
        return new ProductSubscriptionPlanDto(
            subscriptionPlan.Id,
            subscriptionPlan.Title,
            0,
            subscriptionPlan.Trial.HasTrial ? subscriptionPlan.Trial.ToDto() : null);
    }

    public static ProductSubscriptionPlanTrialDto ToDto(this Domain.Entities.TrialPeriod trialPeriod)
    {
        return new ProductSubscriptionPlanTrialDto(trialPeriod.Length, 0);
    }
}
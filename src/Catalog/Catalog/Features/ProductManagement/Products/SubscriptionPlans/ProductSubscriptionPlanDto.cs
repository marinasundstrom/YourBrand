namespace YourBrand.Catalog.Features.ProductManagement.SubscriptionPlans.SubscriptionPlans;

public record ProductSubscriptionPlanDto(string Id, string Name, decimal Price, ProductSubscriptionPlanTrialDto? Trial);

public record ProductSubscriptionPlanTrialDto(int PeriodLength, decimal Price);

public static class Mappings
{
    public static ProductSubscriptionPlanDto ToDto(this Domain.Entities.ProductSubscriptionPlan subscriptionPlan)
    {
        var price = subscriptionPlan.Product.Prices.First();

        return new ProductSubscriptionPlanDto(
            subscriptionPlan.Id,
            subscriptionPlan.Name,
            subscriptionPlan.GetSubscriptionPrice(price.Price),
            subscriptionPlan.Trial.HasTrial ? subscriptionPlan.Trial.ToDto(subscriptionPlan.Product) : null);
    }

    public static ProductSubscriptionPlanTrialDto ToDto(this Domain.Entities.TrialPeriod trialPeriod, Domain.Entities.Product product)
    {
        var price = product.Prices.First();

        return new ProductSubscriptionPlanTrialDto(trialPeriod.Length, trialPeriod.GetTrialPrice(price.Price));
    }
}
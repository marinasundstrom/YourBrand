using System.ComponentModel.DataAnnotations;

using YourBrand.Catalog;

namespace YourBrand.Sales.Subscriptions;

public class SubscriptionViewModel
{
    [Required]
    public Product Product { get; set; }

    [Required]
    public SubscriptionPlan SubscriptionPlan { get; set; }

    public DateTime? StartDate { get; set; } = DateTime.Now;

    public TimeSpan? StartTime { get; set; }

    [Required]
    public Customers.Client.Customer Customer { get; set; }

    public decimal Price { get; set; }

    public string? Notes { get; set; }
}
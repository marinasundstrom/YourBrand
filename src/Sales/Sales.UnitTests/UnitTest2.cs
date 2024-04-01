using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Domain.Enums;
using YourBrand.Sales.Features.Subscriptions;

namespace YourBrand.Carts.UnitTests;

public class UnitTest2
{
    [Fact]
    public void SumOfOrderLines()
    {
        Order order = new()
        {
            Customer = new Customer
            {
                Id = "foobar",
                CustomerNo = 1337,
                Name = "ACME"
            },
            BillingDetails = new Sales.Domain.ValueObjects.BillingDetails()
            {
                FirstName = "Test",
                LastName = "Testsson",
                SSN = "12345",
                Email = "test@email.com",
                Address = new Sales.Domain.ValueObjects.Address
                {
                    Thoroughfare = "Testgatan",
                    Premises = "1",
                    PostalCode = "12345",
                    Locality = "Teststad",
                    SubAdministrativeArea = "Testkommun",
                    AdministrativeArea = "Testregion",
                    Country = "Testland"
                }
            },
            ShippingDetails = new Sales.Domain.ValueObjects.ShippingDetails()
            {
                FirstName = "Test",
                LastName = "Testsson",
                Address = new Sales.Domain.ValueObjects.Address
                {
                    Thoroughfare = "Testgatan",
                    Premises = "1",
                    PostalCode = "12345",
                    Locality = "Teststad",
                    SubAdministrativeArea = "Testkommun",
                    AdministrativeArea = "Testregion",
                    Country = "Testland"
                }
            }
        };

        var item = order.AddItem("Item 1", null, 250m, null, null, null, 2, null, 0.25, null);

        var subscriptionPlan = SubscriptionPlanFactory
                          .CreateWeeklyPlan(1, WeekDays.Tuesday | WeekDays.Thursday, TimeSpan.Parse("16:00"), null)
                          .WithName("Bi-weekly subscription")
                          .WithEndTime(TimeSpan.Parse("17:00"));

        var subscription = new Subscription()
        {
            SubscriptionPlan = subscriptionPlan,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddMonths(12),
            Status = SubscriptionStatus.Active,
            StatusDate = DateTime.Now
        };

        item.Subscription = subscription;

        var subscriptionOrderGenerator = new SubscriptionOrderGenerator(
            new Sales.Features.Orders.OrderFactory(),
            new SubscriptionOrderDateGenerator()
        );

        var orders = subscriptionOrderGenerator.GetOrders(order, DateTime.Now, DateTime.Now.AddMonths(12)).ToList();
    }
}
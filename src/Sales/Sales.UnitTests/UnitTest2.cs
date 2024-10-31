using NSubstitute;

using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Domain.Entities.Builders;
using YourBrand.Sales.Domain.Enums;
using YourBrand.Sales.Features.SubscriptionManagement;

namespace YourBrand.Sales.UnitTests;

public class UnitTest2
{
    [Fact]
    public void GenerateSubscriptions()
    {
        var timeProvider = Substitute.For<TimeProvider>();
        timeProvider.GetUtcNow().Returns(DateTimeOffset.UtcNow);

        Organization organization = new Organization("id", "TestOrg");

        var orderBuilder = OrderBuilder.NewOrder(organization.Id, 1, "SEK", true)
            .WithCustomer(new Customer
            {
                Id = "foobar",
                CustomerNo = 1337,
                Name = "ACME"
            })
            .WithBillingDetails(new Sales.Domain.ValueObjects.BillingDetails()
            {
                FirstName = "Test",
                LastName = "Testsson",
                SSN = "12345",
                Email = "test@email.com",
                Address = new Sales.Domain.ValueObjects.Address
                {
                    Street = "Testgatan 1",
                    PostalCode = "12345",
                    City = "Teststad",
                }
            })
            .WithShippingDetails(new Sales.Domain.ValueObjects.ShippingDetails()
            {
                FirstName = "Test",
                LastName = "Testsson",
                Address = new Sales.Domain.ValueObjects.Address
                {
                    Street = "Testgatan 1",
                    PostalCode = "12345",
                    City = "Teststad",
                }
            });

        Order order = orderBuilder.Build();

        var item = order.AddItem("Item 1", null, 250m, null, null, null, 2, null, 0.25, null, timeProvider);

        var subscriptionPlan = SubscriptionPlan.Create(SubscriptionPlanType.RecurringOrder, "Bi-weekly subscription")
            .WithSchedule(SubscriptionSchedule.Weekly(1, WeekDays.Tuesday | WeekDays.Thursday)
            .WithStartTime(TimeOnly.Parse("16:00"))
            .WithEndTime(TimeOnly.Parse("17:00")));

        var subscription = new Subscription()
        {
            Plan = subscriptionPlan,
            StartDate = DateOnly.FromDateTime(DateTime.Now),
            EndDate = DateOnly.FromDateTime(DateTime.Now).AddMonths(12),
            //Status = SubscriptionStatus.Active,
            StatusDate = DateTime.Now
        };

        item.Subscription = subscription;

        var subscriptionOrderGenerator = new SubscriptionOrderGenerator(
            new Sales.Features.Orders.OrderFactory(timeProvider),
            new SubscriptionOrderDateGenerator(),
            timeProvider
        );

        var orders = subscriptionOrderGenerator.GenerateOrders(order, DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now).AddMonths(12)).ToList();
    }
}
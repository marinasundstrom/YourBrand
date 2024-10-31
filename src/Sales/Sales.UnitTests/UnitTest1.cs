using NSubstitute;

using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Domain.Entities.Builders;

namespace YourBrand.Sales.UnitTests;

public class UnitTest1
{
    [Fact]
    public void SumOfOrderLines()
    {
        var timeProvider = Substitute.For<TimeProvider>();
        timeProvider.GetUtcNow().Returns(DateTimeOffset.UtcNow);

        Organization organization = new Organization("id", "TestOrg");

        var order = OrderBuilder.NewOrder(organization.Id, 1, "SEK", true).Build();

        order.AddItem("Item 1", null, 250m, null, null, null, 3, null, 0.25, null, timeProvider);

        order.AddItem("Item 1", null, 250m, null, null, null, 1, null, 0.14, null, timeProvider);

        var sumOfOrderItemVat = order.Items.Sum(x => x.Vat);

        Assert.Equal(sumOfOrderItemVat, order.Vat);

        var sumOfOrderItemTotals = order.Items.Sum(x => x.Total);

        Assert.Equal(sumOfOrderItemTotals, order.Total);
    }
}
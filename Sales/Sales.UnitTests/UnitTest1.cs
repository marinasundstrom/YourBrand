using YourBrand.Sales.API.Features.OrderManagement.Domain.Entities;

namespace YourBrand.Carts.UnitTests;

public class UnitTest1
{
    [Fact]
    public void SumOfOrderLines()
    {
        Order order = new()
        {

        };

        order.AddItem(null, "Item 1", 3, null, 100m, 0.25, null, null, null, null);

        order.AddItem(null, "Item 2", 1, null, 100m, 0.14, null, null, null, null);

        var sumOfOrderItemVat = order.Items.Sum(x => x.Vat);

        Assert.Equal(sumOfOrderItemVat, order.Vat);

        var sumOfOrderItemTotals = order.Items.Sum(x => x.Total);

        Assert.Equal(sumOfOrderItemTotals, order.Total);
    }
}
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

        order.AddItem("Item 1", null, 250m, null, null, null, 3, null, 0.25, null);

        order.AddItem("Item 1", null, 250m, null, null, null, 1, null, 0.14, null);

        var sumOfOrderItemVat = order.Items.Sum(x => x.Vat);

        Assert.Equal(sumOfOrderItemVat, order.Vat);

        var sumOfOrderItemTotals = order.Items.Sum(x => x.Total);

        Assert.Equal(sumOfOrderItemTotals, order.Total);
    }
}
﻿using YourBrand.Sales.Domain.Entities;

namespace YourBrand.Sales.UnitTests;

public class UnitTest1
{
    [Fact]
    public void SumOfOrderLines()
    {
        Organization organization = new Organization("id", "TestOrg");

        Order order = Order.Create(organization.Id);

        order.AddItem("Item 1", null, 250m, null, null, null, 3, null, 0.25, null);

        order.AddItem("Item 1", null, 250m, null, null, null, 1, null, 0.14, null);

        var sumOfOrderItemVat = order.Items.Sum(x => x.Vat);

        Assert.Equal(sumOfOrderItemVat, order.Vat);

        var sumOfOrderItemTotals = order.Items.Sum(x => x.Total);

        Assert.Equal(sumOfOrderItemTotals, order.Total);
    }
}
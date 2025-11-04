using System;

namespace YourBrand.Inventory.Domain.Entities;

public class WarehouseItemTest
{
    [Fact]
    public void AdjustQuantityOnHand()
    {
        var item = new WarehouseItem("ts-b-l", "wh1", "test", 100);
        item.AdjustQuantityOnHand(98);

        item.QuantityOnHand.ShouldBe(98);
    }

    [Fact]
    public void ReserveQuantity()
    {
        var item = new WarehouseItem("ts-b-l", "wh1", "test", 100);
        item.Reserve(20);

        item.QuantityReserved.ShouldBe(20);
        item.QuantityOnHand.ShouldBe(100);
        item.QuantityAvailable.ShouldBe(80);
    }

    [Fact]
    public void PickQuantity()
    {
        var item = new WarehouseItem("ts-b-l", "wh1", "test", 100);
        item.Pick(20);

        item.QuantityPicked.ShouldBe(20);
        item.QuantityOnHand.ShouldBe(80);
    }

    [Fact]
    public void ShipQuantity()
    {
        var item = new WarehouseItem("ts-b-l", "wh1", "test", 100);
        item.Pick(20);
        item.Ship(20, true);

        item.QuantityPicked.ShouldBe(0);
        item.QuantityOnHand.ShouldBe(80);
    }

    [Fact]
    public void ShippingDirectlyFromShelfReducesOnHandAndReservations()
    {
        var item = new WarehouseItem("ts-b-l", "wh1", "test", 100);
        item.Reserve(30);

        item.Ship(10);

        item.QuantityOnHand.ShouldBe(90);
        item.QuantityReserved.ShouldBe(20);
        item.QuantityAvailable.ShouldBe(70);
    }

    [Fact]
    public void ReserveCannotExceedAvailability()
    {
        var item = new WarehouseItem("ts-b-l", "wh1", "test", 10);

        Should.Throw<InvalidOperationException>(() => item.Reserve(11));
    }

    [Fact]
    public void PickCannotExceedOnHand()
    {
        var item = new WarehouseItem("ts-b-l", "wh1", "test", 5);

        Should.Throw<InvalidOperationException>(() => item.Pick(6));
    }

    [Fact]
    public void PickFromReservedCannotExceedReservedAmount()
    {
        var item = new WarehouseItem("ts-b-l", "wh1", "test", 10);
        item.Reserve(5);

        Should.Throw<InvalidOperationException>(() => item.Pick(6, true));
    }

    [Fact]
    public void ShipCannotExceedPickedQuantity()
    {
        var item = new WarehouseItem("ts-b-l", "wh1", "test", 10);
        item.Pick(4);

        Should.Throw<InvalidOperationException>(() => item.Ship(5, true));
    }
}
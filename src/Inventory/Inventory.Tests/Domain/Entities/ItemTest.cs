using System;

using Shouldly;
using Xunit;

using YourBrand.Inventory.Domain.Enums;

namespace YourBrand.Inventory.Domain.Entities;

public class WarehouseItemTest
{
    [Fact]
    public void AdjustQuantityOnHand()
    {
        var item = CreateWarehouseItem(100);
        item.AdjustQuantityOnHand(98);

        item.QuantityOnHand.ShouldBe(98);
    }

    [Fact]
    public void ReserveQuantity()
    {
        var item = CreateWarehouseItem(100);
        var reservation = item.Reserve(20, TimeSpan.FromMinutes(5));

        item.QuantityReserved.ShouldBe(20);
        item.QuantityOnHand.ShouldBe(100);
        item.QuantityAvailable.ShouldBe(80);
        reservation.ShouldNotBeNull();
        reservation.Status.ShouldBe(WarehouseItemReservationStatus.Pending);
        reservation.RemainingQuantity.ShouldBe(20);
        reservation.ExpiresAt.ShouldBeGreaterThan(DateTimeOffset.UtcNow);
    }

    [Fact]
    public void ExpiredReservationIsReleased()
    {
        var item = CreateWarehouseItem(50);
        var reservation = item.Reserve(10, TimeSpan.FromMinutes(5));

        item.ReleaseReservation(reservation.Id, WarehouseItemReservationStatus.Expired);

        item.QuantityReserved.ShouldBe(0);
        item.QuantityAvailable.ShouldBe(50);
        reservation.Status.ShouldBe(WarehouseItemReservationStatus.Expired);
        reservation.RemainingQuantity.ShouldBe(0);
    }

    [Fact]
    public void PickQuantity()
    {
        var item = CreateWarehouseItem(100);
        item.Pick(20);

        item.QuantityPicked.ShouldBe(20);
        item.QuantityOnHand.ShouldBe(80);
    }

    [Fact]
    public void ShipQuantity()
    {
        var item = CreateWarehouseItem(100);
        item.Pick(20);
        item.Ship(20, true);

        item.QuantityPicked.ShouldBe(0);
        item.QuantityOnHand.ShouldBe(80);
    }

    [Fact]
    public void ShippingDirectlyFromShelfReducesOnHandAndReservations()
    {
        var item = CreateWarehouseItem(100);
        item.Reserve(30);

        item.Ship(10);

        item.QuantityOnHand.ShouldBe(90);
        item.QuantityReserved.ShouldBe(20);
        item.QuantityAvailable.ShouldBe(70);
    }

    [Fact]
    public void ReserveCannotExceedAvailability()
    {
        var item = CreateWarehouseItem(10);

        Should.Throw<InvalidOperationException>(() => item.Reserve(11));
    }

    [Fact]
    public void PickCannotExceedOnHand()
    {
        var item = CreateWarehouseItem(5);

        Should.Throw<InvalidOperationException>(() => item.Pick(6));
    }

    [Fact]
    public void PickFromReservedCannotExceedReservedAmount()
    {
        var item = CreateWarehouseItem(10);
        item.Reserve(5);

        Should.Throw<InvalidOperationException>(() => item.Pick(6, true));
    }

    [Fact]
    public void ShipCannotExceedPickedQuantity()
    {
        var item = CreateWarehouseItem(10);
        item.Pick(4);

        Should.Throw<InvalidOperationException>(() => item.Ship(5, true));
    }

    private static WarehouseItem CreateWarehouseItem(int quantity)
    {
        var site = new Site(Guid.NewGuid().ToString(), "Site");
        var warehouse = new Warehouse(Guid.NewGuid().ToString(), "Warehouse", site);
        var group = new ItemGroup("Group");
        var item = new Item("ts-b-l", "Test", ItemType.Inventory, "gtin", group.Id, "pcs");
        item.SetGroup(group);

        return new WarehouseItem(item, warehouse, "A-1", quantity);
    }
}

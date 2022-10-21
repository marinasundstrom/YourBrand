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
        item.QuantityOnHand.ShouldBe(50);
    }

    [Fact]
    public void PickQuantity()
    {
        var item = new WarehouseItem("ts-b-l", "wh1", "test", 100);
        item.Pick(20);

        item.QuantityPicked.ShouldBe(20);
        item.QuantityOnHand.ShouldBe(30);
    }

    [Fact]
    public void ShipQuantity()
    {
        var item = new WarehouseItem("ts-b-l", "wh1", "test", 100);
        item.Pick(20);
        item.Ship(20, true);

        item.QuantityPicked.ShouldBe(0);
    }
}

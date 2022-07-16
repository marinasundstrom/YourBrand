namespace YourBrand.Warehouse.Domain.Entities;

public class ItemTest
{
    [Fact]
    public void AdjustQuantityOnHand()
    {
        var item = new Item("ts-b-l", "T-Shirt Blue Large", 100);
        item.AdjustQuantityOnHand(98);

        item.QuantityOnHand.ShouldBe(98);
    }

    [Fact]
    public void ReserveQuantity()
    {
        var item = new Item("ts-b-l", "T-Shirt Blue Large", 50);
        item.Reserve(20);

        item.QuantityReserved.ShouldBe(20);
        item.QuantityOnHand.ShouldBe(50);
    }

    [Fact]
    public void PickQuantity()
    {
        var item = new Item("ts-b-l", "T-Shirt Blue Large", 50);
        item.Pick(20);

        item.QuantityPicked.ShouldBe(20);
        item.QuantityOnHand.ShouldBe(30);
    }

    [Fact]
    public void ShipQuantity()
    {
        var item = new Item("ts-b-l", "T-Shirt Blue Large", 50);
        item.Pick(20);
        item.Ship(20);

        item.QuantityPicked.ShouldBe(0);
    }
}

using System;

using YourBrand.Inventory.Domain.Common;
using YourBrand.Inventory.Domain.Events;

namespace YourBrand.Inventory.Domain.Entities;

public class Item : AuditableEntity
{
    protected Item() { }

    public Item(string id, string name, int quantityOnHand)
    {
        Id = id;
        Name = name;
        QuantityOnHand = quantityOnHand;
    }

    public string Id { get; set; }

    public string Name { get; set; } = null!;

    public string Barcode { get; set; } = null!;

    public ItemGroup Group { get; set; } = null!;

    public string Site { get; set; }

    public string Warehouse { get; set; }

    public string Location { get; set; }
    
    public string Unit { get; set; } = null!;

    /// <summary>
    /// This number is the total you have physically available (including Qty Reserved),
    /// minus any items that have already been “picked” in a sales order (i.e. what’s still on your inventory shelves).
    /// </summary>
    public int QuantityOnHand { get; private set; }

    public void AdjustQuantityOnHand(int quantity)
    {
        QuantityOnHand = quantity;

        AddDomainEvent(new ItemQuantityOnHandUpdated(Id, QuantityOnHand));
        AddDomainEvent(new ItemQuantityAvailableUpdated(Id, QuantityAvailable));
    }

    public void Receive(int quantity)
    {
        QuantityOnHand += quantity;

        AddDomainEvent(new ItemQuantityOnHandUpdated(Id, QuantityOnHand));
        AddDomainEvent(new ItemQuantityAvailableUpdated(Id, QuantityAvailable));
    }

    /// <summary>
    /// This number is the total that has already been picked in sales orders/work orders and
    /// are awaiting shipment (think of them as sitting in a box waiting to be shipped out).
    /// </summary>
    public int QuantityPicked { get; private set; }

    public void Pick(int quantity, bool fromReserved = false)
    {
        QuantityPicked += quantity;
        QuantityOnHand -= quantity;

        if(fromReserved) 
        {
            QuantityReserved -= quantity;
            //AddDomainEvent(new ItemQuantityReservedUpdated(Id, quantity));
        }

        AddDomainEvent(new ItemsPicked(Id, quantity));
        AddDomainEvent(new ItemQuantityOnHandUpdated(Id, QuantityOnHand));
        AddDomainEvent(new ItemQuantityAvailableUpdated(Id, QuantityAvailable));
    }

    public void Ship(int quantity, bool fromPicked = false)
    {
        //QuantityOnHand -= quantity;

        if(fromPicked) 
        {
            QuantityPicked -= quantity;
            AddDomainEvent(new ItemsPicked(Id, quantity));
        }

        //AddDomainEvent(new ItemQuantityOnHandUpdated(Id, QuantityOnHand));
        AddDomainEvent(new ItemQuantityAvailableUpdated(Id, QuantityAvailable));
    }

    /// <summary>
    /// This number is the total ordered by your customers (across all open sales orders/work orders).
    /// This number is what you need to be fulfilling to complete sales/production!
    /// </summary>
    public int QuantityReserved { get; private set; }

    public void Reserve(int quantity)
    {
        QuantityReserved += quantity;

        AddDomainEvent(new ItemsReserved(Id, quantity));
        AddDomainEvent(new ItemQuantityAvailableUpdated(Id, QuantityAvailable));
    }

    /// <summary>
    /// This number is how many of the item you have left if you fulfill all open sales orders and is therefore equal to Qty on Hand – Qty Reserved.
    /// (i.e. what’s left after you’ve shipped all your current orders).
    /// </summary>
    public int QuantityAvailable => QuantityOnHand - QuantityReserved;
}

using System;
using System.Collections.Generic;
using System.Linq;

using YourBrand.Inventory.Domain.Common;
using YourBrand.Inventory.Domain.Enums;
using YourBrand.Inventory.Domain.Events;

namespace YourBrand.Inventory.Domain.Entities;

public class Item : AuditableEntity<string>
{
    private readonly HashSet<WarehouseItem> warehouseItems = new();
    private readonly HashSet<SupplierItem> supplierItems = new();

    protected Item() { }

    public Item(string id, string name, ItemType type, string gtin, string groupId, string unit)
        : base(id)
    {
        Rename(name);
        ChangeType(type);
        UpdateGtin(gtin);
        ChangeGroup(groupId);
        ChangeUnit(unit);
        RecalculateAvailability();
    }

    public string Name { get; private set; } = null!;

    public ItemType Type { get; private set; }

    public string? GTIN { get; private set; }

    public ItemGroup Group { get; private set; } = null!;

    public string GroupId { get; private set; } = null!;

    public string Unit { get; private set; } = null!;

    public ItemAvailability Availability { get; private set; } = ItemAvailability.SupplierOutOfStock;

    public bool Discontinued { get; private set; }

    public int QuantityAvailable => warehouseItems.Sum(x => x.QuantityAvailable);

    public IReadOnlyCollection<WarehouseItem> WarehouseItems => warehouseItems;

    public IReadOnlyCollection<SupplierItem> SupplierItems => supplierItems;

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        }

        Name = name.Trim();
    }

    public void ChangeType(ItemType type)
    {
        Type = type;
    }

    public void UpdateGtin(string? gtin)
    {
        if (string.IsNullOrWhiteSpace(gtin))
        {
            GTIN = null;
            return;
        }

        GTIN = gtin.Trim();
    }

    public void ChangeGroup(string groupId)
    {
        if (string.IsNullOrWhiteSpace(groupId))
        {
            throw new ArgumentException("Group id cannot be empty.", nameof(groupId));
        }

        GroupId = groupId.Trim();
    }

    public void SetGroup(ItemGroup group)
    {
        Group = group ?? throw new ArgumentNullException(nameof(group));
        ChangeGroup(group.Id);
    }

    public void ChangeUnit(string unit)
    {
        if (string.IsNullOrWhiteSpace(unit))
        {
            throw new ArgumentException("Unit cannot be empty.", nameof(unit));
        }

        Unit = unit.Trim();
    }

    public void MarkDiscontinued()
    {
        Discontinued = true;
    }

    public void Reinstate()
    {
        Discontinued = false;
    }

    internal void AttachWarehouseItem(WarehouseItem warehouseItem)
    {
        if (!warehouseItems.Contains(warehouseItem))
        {
            warehouseItems.Add(warehouseItem);
            RecalculateAvailability();
        }
    }

    internal void DetachWarehouseItem(WarehouseItem warehouseItem)
    {
        if (warehouseItems.Remove(warehouseItem))
        {
            RecalculateAvailability();
        }
    }

    internal void AttachSupplierItem(SupplierItem supplierItem)
    {
        if (!supplierItems.Contains(supplierItem))
        {
            supplierItems.Add(supplierItem);
        }
    }

    internal void DetachSupplierItem(SupplierItem supplierItem)
    {
        supplierItems.Remove(supplierItem);
    }

    internal void RecalculateAvailability()
    {
        var previous = Availability;

        if (!warehouseItems.Any())
        {
            Availability = ItemAvailability.SupplierOutOfStock;
        }
        else if (warehouseItems.Any(i => i.Availability == WarehouseItemAvailability.InStock))
        {
            Availability = ItemAvailability.InStock;
        }
        else if (warehouseItems.Any(i => i.Availability == WarehouseItemAvailability.StockedOnDemand))
        {
            Availability = ItemAvailability.StockedOnDemand;
        }
        else
        {
            Availability = ItemAvailability.SupplierOutOfStock;
        }

        if (previous != Availability)
        {
            // Availability changes are significant for downstream processes; surface a domain event.
            AddDomainEvent(new ItemAvailabilityChanged(Id, previous, Availability));
        }
    }
}
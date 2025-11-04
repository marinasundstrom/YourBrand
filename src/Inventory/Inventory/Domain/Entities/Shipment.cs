using System;
using System.Collections.Generic;
using System.Linq;

using YourBrand.Inventory.Domain.Common;
using YourBrand.Inventory.Domain.ValueObjects;

namespace YourBrand.Inventory.Domain.Entities;

public class Shipment : AuditableEntity<string>
{
    private readonly HashSet<ShipmentItem> items = new();

    protected Shipment() { }

    public Shipment(Warehouse warehouse, string orderNo, ShippingDetails destination, string service)
        : base(Guid.NewGuid().ToString())
    {
        Warehouse = warehouse ?? throw new ArgumentNullException(nameof(warehouse));
        WarehouseId = warehouse.Id;

        SetOrderNo(orderNo);
        ChangeDestination(destination);
        ChangeService(service);
    }

    public string WarehouseId { get; private set; } = null!;

    public Warehouse Warehouse { get; private set; } = null!;

    public string OrderNo { get; private set; } = null!;

    public ShippingDetails Destination { get; private set; } = null!;

    public string Service { get; private set; } = null!;

    public DateTimeOffset? ShippedAt { get; private set; }

    public bool IsShipped => ShippedAt.HasValue;

    public IReadOnlyCollection<ShipmentItem> Items => items;

    public void SetOrderNo(string orderNo)
    {
        if (IsShipped)
        {
            throw new InvalidOperationException("Cannot modify a shipment that has already been shipped.");
        }

        if (string.IsNullOrWhiteSpace(orderNo))
        {
            throw new ArgumentException("Order number cannot be empty.", nameof(orderNo));
        }

        OrderNo = orderNo.Trim();
    }

    public void ChangeDestination(ShippingDetails destination)
    {
        if (IsShipped)
        {
            throw new InvalidOperationException("Cannot modify a shipment that has already been shipped.");
        }

        if (destination is null)
        {
            throw new ArgumentNullException(nameof(destination));
        }

        if (string.IsNullOrWhiteSpace(destination.FirstName))
        {
            throw new ArgumentException("Destination first name cannot be empty.", nameof(destination));
        }

        if (string.IsNullOrWhiteSpace(destination.LastName))
        {
            throw new ArgumentException("Destination last name cannot be empty.", nameof(destination));
        }

        if (destination.Address is null)
        {
            throw new ArgumentException("Destination address is required.", nameof(destination));
        }

        if (string.IsNullOrWhiteSpace(destination.Address.Street) || string.IsNullOrWhiteSpace(destination.Address.City) || string.IsNullOrWhiteSpace(destination.Address.PostalCode) || string.IsNullOrWhiteSpace(destination.Address.Country))
        {
            throw new ArgumentException("Destination address must include street, city, postal code, and country.", nameof(destination));
        }

        Destination = destination.Copy();
    }

    public void ChangeService(string service)
    {
        if (IsShipped)
        {
            throw new InvalidOperationException("Cannot modify a shipment that has already been shipped.");
        }

        if (string.IsNullOrWhiteSpace(service))
        {
            throw new ArgumentException("Service cannot be empty.", nameof(service));
        }

        Service = service.Trim();
    }

    public ShipmentItem AddItem(WarehouseItem warehouseItem, int quantity)
    {
        if (warehouseItem is null)
        {
            throw new ArgumentNullException(nameof(warehouseItem));
        }

        if (warehouseItem.WarehouseId != WarehouseId)
        {
            throw new InvalidOperationException("Warehouse item does not belong to the shipment warehouse.");
        }

        if (IsShipped)
        {
            throw new InvalidOperationException("Cannot modify a shipment that has already been shipped.");
        }

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        if (items.Any(x => x.WarehouseItemId == warehouseItem.Id))
        {
            throw new InvalidOperationException("Item has already been added to the shipment.");
        }

        var shipmentItem = new ShipmentItem(this, warehouseItem, quantity);
        items.Add(shipmentItem);

        return shipmentItem;
    }

    public ShipmentItem GetItem(string shipmentItemId)
    {
        var shipmentItem = items.FirstOrDefault(x => x.Id == shipmentItemId);

        if (shipmentItem is null)
        {
            throw new InvalidOperationException("Shipment item was not found on this shipment.");
        }

        return shipmentItem;
    }

    public void UpdateItemQuantity(string shipmentItemId, int quantity)
    {
        if (IsShipped)
        {
            throw new InvalidOperationException("Cannot modify a shipment that has already been shipped.");
        }

        var shipmentItem = GetItem(shipmentItemId);
        shipmentItem.ChangeQuantity(quantity);
    }

    public void RemoveItem(string shipmentItemId)
    {
        if (IsShipped)
        {
            throw new InvalidOperationException("Cannot modify a shipment that has already been shipped.");
        }

        var shipmentItem = GetItem(shipmentItemId);
        shipmentItem.Cancel();
        items.Remove(shipmentItem);
    }

    public void Ship(DateTimeOffset? shippedAt = null)
    {
        if (IsShipped)
        {
            throw new InvalidOperationException("Shipment has already been shipped.");
        }

        if (!items.Any())
        {
            throw new InvalidOperationException("Cannot ship an empty shipment.");
        }

        foreach (var item in items)
        {
            item.Ship(shippedAt);
        }

        ShippedAt = shippedAt ?? DateTimeOffset.UtcNow;
    }

    public void Cancel()
    {
        if (IsShipped)
        {
            throw new InvalidOperationException("Cannot cancel a shipment that has already been shipped.");
        }

        foreach (var item in items.ToList())
        {
            item.Cancel();
            items.Remove(item);
        }
    }

}

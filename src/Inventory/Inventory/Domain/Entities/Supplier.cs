namespace YourBrand.Inventory.Domain.Entities;

using System;
using System.Collections.Generic;
using System.Linq;

using YourBrand.Inventory.Domain.Common;

public class Supplier : AuditableEntity<string>
{
    private readonly HashSet<SupplierItem> items = new();
    private readonly HashSet<SupplierOrder> orders = new();

    protected Supplier() { }

    public Supplier(string id, string name)
        : base(id)
    {
        Rename(name);
    }

    public string Name { get; private set; } = null!;

    public IReadOnlyCollection<SupplierItem> Items => items;

    public IReadOnlyCollection<SupplierOrder> Orders => orders;

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Supplier name cannot be empty.", nameof(name));
        }

        Name = name.Trim();
    }

    public SupplierOrder PlaceOrder(string orderNumber, DateTime orderedAt, DateTime? expectedDelivery = null)
    {
        var order = new SupplierOrder(this, orderNumber, orderedAt, expectedDelivery);
        AttachOrder(order);
        return order;
    }

    internal void AttachItem(SupplierItem supplierItem)
    {
        if (!items.Contains(supplierItem))
        {
            items.Add(supplierItem);
        }
    }

    internal void DetachItem(SupplierItem supplierItem)
    {
        items.Remove(supplierItem);
    }

    internal void AttachOrder(SupplierOrder supplierOrder)
    {
        if (!orders.Contains(supplierOrder))
        {
            orders.Add(supplierOrder);
        }
    }

    internal void DetachOrder(SupplierOrder supplierOrder)
    {
        orders.Remove(supplierOrder);
    }

    public bool SuppliesItem(string itemId) => items.Any(x => x.ItemId == itemId);
}

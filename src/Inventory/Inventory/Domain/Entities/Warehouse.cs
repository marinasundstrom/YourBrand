using System;
using System.Collections.Generic;

using YourBrand.Inventory.Domain.Common;

namespace YourBrand.Inventory.Domain.Entities;

public class Warehouse : AuditableEntity<string>
{
    private readonly HashSet<WarehouseItem> items = new();

    protected Warehouse() { }

    public Warehouse(string id, string name, Site site)
        : base(id)
    {
        Rename(name);
        ChangeSite(site);
    }

    public Warehouse(string id, string name, string siteId)
        : base(id)
    {
        Rename(name);
        SiteId = siteId;
    }

    public string Name { get; private set; } = null!;

    public string SiteId { get; private set; } = null!;

    public Site Site { get; private set; } = null!;

    public bool IsActive { get; private set; } = true;

    public IReadOnlyCollection<WarehouseItem> Items => items;

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Warehouse name cannot be empty.", nameof(name));
        }

        Name = name.Trim();
    }

    public void Retire()
    {
        IsActive = false;
    }

    public void Reactivate()
    {
        IsActive = true;
    }

    public void MoveItemTo(WarehouseItem warehouseItem, Warehouse destination)
    {
        if (warehouseItem is null)
        {
            throw new ArgumentNullException(nameof(warehouseItem));
        }

        if (destination is null)
        {
            throw new ArgumentNullException(nameof(destination));
        }

        if (!items.Contains(warehouseItem))
        {
            throw new InvalidOperationException("Warehouse does not own the specified item.");
        }

        items.Remove(warehouseItem);
        warehouseItem.AssignWarehouse(destination);
    }

    internal void AttachItem(WarehouseItem warehouseItem)
    {
        items.Add(warehouseItem);
    }

    internal void DetachItem(WarehouseItem warehouseItem)
    {
        items.Remove(warehouseItem);
    }

    public void ChangeSite(Site site)
    {
        ChangeSiteInternal(site ?? throw new ArgumentNullException(nameof(site)));
    }

    internal void ChangeSiteInternal(Site site)
    {
        Site?.DetachWarehouse(this);

        Site = site;
        SiteId = site.Id;
        site.AttachWarehouse(this);
    }
}
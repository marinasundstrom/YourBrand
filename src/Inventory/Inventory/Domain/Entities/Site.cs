using System;
using System.Collections.Generic;

using YourBrand.Inventory.Domain.Common;

namespace YourBrand.Inventory.Domain.Entities;

public class Site : AuditableEntity<string>
{
    private readonly HashSet<Warehouse> warehouses = new();

    protected Site() { }

    public Site(string id, string name)
        : base(id)
    {
        Rename(name);
    }

    public string Name { get; private set; } = null!;

    public bool IsActive { get; private set; } = true;

    public IReadOnlyCollection<Warehouse> Warehouses => warehouses;

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Site name cannot be empty.", nameof(name));
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

    internal void AttachWarehouse(Warehouse warehouse)
    {
        warehouses.Add(warehouse);
    }

    internal void DetachWarehouse(Warehouse warehouse)
    {
        warehouses.Remove(warehouse);
    }
}
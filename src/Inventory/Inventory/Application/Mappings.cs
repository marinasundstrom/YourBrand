using System.Linq;

using YourBrand.Inventory.Application.Items;
using YourBrand.Inventory.Application.Items.Groups;
using YourBrand.Inventory.Application.Sites;
using YourBrand.Inventory.Application.Warehouses;
using YourBrand.Inventory.Application.Warehouses.Items;
using YourBrand.Inventory.Application.Warehouses.Shipments;
using YourBrand.Inventory.Application.Suppliers;
using YourBrand.Inventory.Domain.Entities;
using YourBrand.Inventory.Domain.ValueObjects;
using YourBrand.Domain;

namespace YourBrand.Inventory.Application;

public static class Mappings
{
    public static ItemDto ToDto(this Item item)
    {
        return new ItemDto(
            item.Id,
            item.Name,
            (ItemTypeDto)item.Type,
            item.GTIN,
            item.Group.ToDto(),
            item.Unit,
            item.QuantityAvailable,
            item.Discontinued);
    }

    public static ItemGroupDto ToDto(this ItemGroup itemGroup)
    {
        return new ItemGroupDto(
            itemGroup.Id,
            itemGroup.Name);
    }

    public static WarehouseDto ToDto(this Warehouse item)
    {
        return new WarehouseDto(
            item.Id,
            item.Name,
            item.Site.ToDto());
    }

    public static WarehouseItemDto ToDto(this WarehouseItem item)
    {
        return new WarehouseItemDto(
            item.Id,
            item.Item.ToDto(),
            item.Warehouse.ToDto(),
            item.Location.ToString(),
            item.QuantityOnHand,
            item.QuantityPicked,
            item.QuantityReserved,
            item.QuantityAvailable,
            item.QuantityThreshold);
    }

    public static SiteDto ToDto(this Site item)
    {
        return new SiteDto(
            item.Id,
            item.Name);
    }

    public static SupplierDto ToDto(this Supplier supplier)
    {
        return new SupplierDto(
            supplier.Id,
            supplier.Name,
            supplier.Items.Select(x => x.ToDto()).ToList());
    }

    public static SupplierItemDto ToDto(this SupplierItem supplierItem)
    {
        return new SupplierItemDto(
            supplierItem.Id,
            supplierItem.ItemId,
            supplierItem.Item.Name,
            supplierItem.SupplierItemId,
            supplierItem.UnitCost,
            supplierItem.LeadTimeDays);
    }

    public static SupplierOrderDto ToDto(this SupplierOrder order)
    {
        return new SupplierOrderDto(
            order.Id,
            order.SupplierId,
            order.Supplier.Name,
            order.OrderNumber,
            order.OrderedAt,
            order.ExpectedDelivery,
            order.ReceivedAt,
            order.Lines.Select(x => x.ToDto()).ToList(),
            order.TotalQuantityOutstanding);
    }

    public static SupplierOrderLineDto ToDto(this SupplierOrderLine line)
    {
        return new SupplierOrderLineDto(
            line.Id,
            line.SupplierItemId,
            line.SupplierItem.ItemId,
            line.SupplierItem.Item.Name,
            line.QuantityOrdered,
            line.ExpectedQuantity,
            line.QuantityReceived,
            line.QuantityOutstanding,
            line.ExpectedOn);
    }

    public static ShipmentDto ToDto(this Shipment shipment)
    {
        return new ShipmentDto(
            shipment.Id,
            shipment.OrderNo,
            shipment.Warehouse.ToDto(),
            shipment.Destination.ToDto(),
            shipment.Service,
            shipment.Created,
            shipment.ShippedAt,
            shipment.Items.Select(x => x.ToDto()).ToList());
    }

    public static ShipmentItemDto ToDto(this ShipmentItem shipmentItem)
    {
        return new ShipmentItemDto(
            shipmentItem.Id,
            shipmentItem.WarehouseItem.ToDto(),
            shipmentItem.Quantity,
            shipmentItem.IsShipped,
            shipmentItem.ShippedAt);
    }

    public static ShippingDetailsDto ToDto(this ShippingDetails destination)
    {
        return new ShippingDetailsDto(
            destination.FirstName,
            destination.LastName,
            destination.CareOf,
            destination.Address.ToDto());
    }

    public static ShippingDetails ToValueObject(this ShippingDetailsDto destination)
    {
        if (destination.Address is null)
        {
            throw new ArgumentException("Destination address is required.", nameof(destination));
        }

        return new ShippingDetails
        {
            FirstName = destination.FirstName,
            LastName = destination.LastName,
            CareOf = destination.CareOf,
            Address = destination.Address.ToAddress()
        };
    }
}
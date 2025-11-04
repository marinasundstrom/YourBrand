using System.Linq;

using YourBrand.Inventory.Application.Items;
using YourBrand.Inventory.Application.Items.Groups;
using YourBrand.Inventory.Application.Sites;
using YourBrand.Inventory.Application.Warehouses;
using YourBrand.Inventory.Application.Warehouses.Items;
using YourBrand.Inventory.Application.Suppliers;
using YourBrand.Inventory.Domain.Entities;

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
            item.QuantityThreshold,
            item.Reservations
                .OrderBy(r => r.ReservedAt)
                .Select(r => r.ToDto())
                .ToList());
    }

    public static WarehouseItemReservationDto ToDto(this WarehouseItemReservation reservation)
    {
        return new WarehouseItemReservationDto(
            reservation.Id,
            reservation.Quantity,
            reservation.ConsumedQuantity,
            reservation.ReleasedQuantity,
            reservation.RemainingQuantity,
            (WarehouseItemReservationStatusDto)reservation.Status,
            reservation.ReservedAt,
            reservation.ExpiresAt,
            reservation.ConfirmedAt,
            reservation.ReleasedAt,
            reservation.Reference);
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
}
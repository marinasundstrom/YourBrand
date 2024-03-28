using YourBrand.Inventory.Domain.Entities;
using YourBrand.Inventory.Application.Items;
using YourBrand.Inventory.Application.Items.Groups;
using YourBrand.Inventory.Application.Sites;
using YourBrand.Inventory.Application.Warehouses;
using YourBrand.Inventory.Application.Warehouses.Items;

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
            item.Location,
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
}
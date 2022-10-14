using YourBrand.Warehouse.Domain.Entities;
using YourBrand.Warehouse.Application.Items;

namespace YourBrand.Warehouse.Application;

public static class Mappings 
{
    public static ItemDto ToDto(this Item item) 
    {
        return new ItemDto(
            item.Id,
            item.Name,
            item.QuantityOnHand,
            item.QuantityPicked,
            item.QuantityReserved,
            item.QuantityAvailable);
    }
}
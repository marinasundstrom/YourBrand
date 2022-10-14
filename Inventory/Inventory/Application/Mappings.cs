using YourBrand.Inventory.Domain.Entities;
using YourBrand.Inventory.Application.Items;

namespace YourBrand.Inventory.Application;

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
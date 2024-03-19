using Microsoft.VisualBasic;

using YourBrand.Sales;

using static YourBrand.Sales.OrderManagement.OrderItemViewModel;

namespace YourBrand.Sales.OrderManagement;

public static class Mapper
{
    public static OrderViewModel ToModel(this YourBrand.Sales.Order dto)
    {
        var model = new OrderViewModel
        {
            Id = dto.OrderNo,
            Date = dto.Date.Date,
            Status = dto.Status,
        };

        foreach (var item in dto.Items)
        {
            model.AddItem(item.ToModel());
        }

        return model;
    }

    public static OrderItemViewModel ToModel(this OrderItem dto)
    {
        return new OrderItemViewModel
        {
            Id = dto.Id,
            ItemId = dto.ItemId,
            Description = dto.Description,
            Unit = dto.Unit ?? string.Empty,
            Quantity = dto.Quantity,
            Price = dto.UnitPrice,
            RegularPrice = dto.RegularPrice,
            //SubTotal = dto.SubTotal,
            VatRate = dto.VatRate.GetValueOrDefault(),
            Notes = dto.Notes
        };
    }

    public static AddOrderItemRequest ToCreateOrderItemRequest(this OrderItemViewModel vm)
    {
        return new AddOrderItemRequest
        {
            ItemId = vm.ItemId,
            Description = vm.Description,
            Unit = vm.Unit,
            Quantity = vm.Quantity,
            UnitPrice = vm.Price,
            RegularPrice = vm.RegularPrice,
            Discount = vm.Discount,
            VatRate = vm.VatRate.GetValueOrDefault(),
            Notes = vm.Notes,
        };
    }

    public static UpdateOrderItemRequest ToUpdateOrderItemRequest(this OrderItemViewModel dto)
    {
        return new UpdateOrderItemRequest
        {
            Description = dto.Description,
            ItemId = dto.ItemId,
            Notes = dto.Notes,
            UnitPrice = dto.Price,
            RegularPrice = dto.RegularPrice,
            Unit = dto.Unit,
            Quantity = dto.Quantity,
            VatRate = dto.VatRate.GetValueOrDefault(),
        };
    }
}

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
            ItemId = dto.ProductId,
            Description = dto.Description,
            Product = dto.ProductId is null ? null : new Catalog.Product 
            {
                Id = 1,
                Name = dto.Description
            },
            SubscriptionPlan = dto.SubscriptionPlan,
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
            ItemId = vm.Product?.Id.ToString(),
            Description = vm.Description,
            SubscriptionPlanId = vm.SubscriptionPlan?.Id,
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
            ItemId = dto.Product?.Id.ToString(),
            SubscriptionPlanId = dto.SubscriptionPlan?.Id,
            Notes = dto.Notes,
            UnitPrice = dto.Price,
            RegularPrice = dto.RegularPrice,
            Unit = dto.Unit,
            Quantity = dto.Quantity,
            VatRate = dto.VatRate.GetValueOrDefault(),
        };
    }
}

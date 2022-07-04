using System;
using System.Linq;

using OrderPriceCalculator;

using YourBrand.Orders.Contracts;
using YourBrand.Orders.Domain.Entities;

namespace YourBrand.Orders
{
    public static class Mappings
    {
        public static OrderDto CreateOrderDto(this Order order)
        {
            var dto = new OrderDto()
            {
                Id = order.Id,
                OrderNo = order.OrderNo,
                Status = CreateOrderStatusDto(order.Status),
                OrderDate = order.OrderDate,
                Items = order.Items.OrderBy(i => i.Created).Select(CreateOrderItemDto),
                Totals = order.Vat(),
                SubTotal = order.SubTotal.GetValueOrDefault(),
                Vat = order.Vat.GetValueOrDefault(),
                VatRate = order.VatRate,
                Charges = order.Charges.Select(CreateOrderChargeDto),
                Charge = order.Charge,
                Discounts = order.Discounts.Select(CreateOrderDiscountDto),
                Discount = order.Discount,
                Rounding = order.Rounding,
                Total = order.Total,
                CustomFields = order.CustomFields.ToDictionary(f => f.CustomFieldId, f => (object)ConvertValue(f.Value))
            };

            return dto;
        }

        public static object ConvertValue(string v)
        {
            if (bool.TryParse(v, out var boolValue))
            {
                return boolValue;
            }

            if (double.TryParse(v, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var doubleValue))
            {
                return doubleValue;
            }

            if (int.TryParse(v, out var intValue))
            {
                return intValue;
            }

            return v;
        }

        public static OrderStatusDto CreateOrderStatusDto(this OrderStatus status)
        {
            var dto = new OrderStatusDto
            {
                Id = status.Id,
                Name = status.Name
            };

            return dto;
        }

        public static OrderItemDto CreateOrderItemDto(this OrderItem i)
        {
            var dto = new OrderItemDto
            {
                Id = i.Id,
                Description = i.Description,
                ItemId = i.ItemId,
                Unit = i.Unit is not null ? new YourBrand.Orders.Contracts.UnitDto
                {
                    Id = 0,
                    Name = "piece",
                    Code = i.Unit,
                } : null,
                Price = i.Price,
                VatRate = i.VatRate,
                Quantity = i.Quantity,
                Charges = i.Charges.Select(CreateOrderChargeDto),
                Charge = i.Charge,
                Discounts = i.Discounts.Select(CreateOrderDiscountDto),
                Discount = i.Discount,
                Total = i.Total,
                CustomFields = i.CustomFields.ToDictionary(f => f.CustomFieldId, f => (object)ConvertValue(f.Value))
            };

            return dto;
        }

        public static OrderChargeDto CreateOrderChargeDto(this OrderCharge arg)
        {
            var chargeDto = new OrderChargeDto
            {
                Id = arg.Id,
                Amount = arg.Amount,
                Percent = arg.Percent,
                Description = arg.Description,
                ChargeId = arg.ChargeId
            };

            if (arg.Order is not null)
            {
                chargeDto.Total = arg.Total;
            }
            else if (arg.OrderItem is not null)
            {
                chargeDto.Total = arg.Total;
            }

            return chargeDto;
        }


        public static OrderDiscountDto CreateOrderDiscountDto(this OrderDiscount arg)
        {
            var discountDto = new OrderDiscountDto
            {
                Id = arg.Id,
                Quantity = arg.Quantity,
                Limit = arg.Limit,
                Amount = arg.Amount,
                Percent = arg.Percent,
                Description = arg.Description,
                DiscountId = arg.DiscountId
            };

            if (arg.Percent is not null)
            {
                if (arg.Order is not null)
                {
                    discountDto.Total = arg.Total;
                }
                else if (arg.OrderItem is not null)
                {
                    discountDto.Total = arg.Total;
                }
            }

            return discountDto;
        }

    }
}
using System;
using System.Collections.Generic;

namespace YourBrand.Orders.Contracts
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public int OrderNo { get; set; }
        public DateTime? OrderDate { get; set; }
        public OrderStatusDto Status { get; set; } = null!;
        public IEnumerable<OrderItemDto> Items { get; set; } = null!;
        public IDictionary<string, OrderTotalDto>? Totals { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Vat { get; set; }
        public double? VatRate { get; set; }
        public IEnumerable<OrderChargeDto> Charges { get; set; } = null!;
        public decimal? Charge { get; set; }
        public IEnumerable<OrderDiscountDto> Discounts { get; set; } = null!;
        public decimal? Discount { get; set; }
        public decimal? Rounding { get; set; }
        public decimal Total { get; set; }
        public Dictionary<string, object> CustomFields { get; set; } = null!;
    }

    public class OrderStatusDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
    }

    public class OrderItemDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = null!;
        public string? ItemId { get; set; }
        public UnitDto? Unit { get; set; }
        public decimal Price { get; set; }
        public double VatRate { get; set; }
        public double Quantity { get; set; }
        public IEnumerable<OrderChargeDto> Charges { get; set; } = null!;
        public decimal? Charge { get; set; }
        public IEnumerable<OrderDiscountDto> Discounts { get; set; } = null!;
        public decimal? Discount { get; set; }
        public decimal Total { get; set; }
        public Dictionary<string, object> CustomFields { get; set; } = null!;
    }

    public class UnitDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Code { get; set; }
    }

    public class OrderTotalDto
    {
        public OrderTotalDto(decimal subTotal, decimal vat, decimal total)
        {
            this.SubTotal = subTotal;
            this.Vat = vat;
            this.Total = total;
        }

        public decimal SubTotal { get; set; }
        public decimal Vat { get; set; }
        public decimal Total { get; set; }
    }

    public class OrderTotalsDto
    {
        public IDictionary<string, OrderTotalDto> Totals { get; set; } = null!;
        public decimal SubTotal { get; set; }
        public decimal Vat { get; set; }
        public IEnumerable<OrderDiscountDto> Discounts { get; set; } = null!;
        public decimal? Rounding { get; set; }
        public decimal Total { get; set; }
    }

    public class OrderItemProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
    }

    public class OrderChargeDto
    {
        public Guid Id { get; set; }
        public int? Quantity { get; set; }
        public int? Limit { get; set; }
        public decimal? Amount { get; set; }
        public double? Percent { get; set; }
        public decimal Total { get; set; }
        public string Description { get; set; } = null!;
        public Guid? ChargeId { get; set; }
    }

    public class OrderDiscountDto
    {
        public Guid Id { get; set; }
        public int? Quantity { get; set; }
        public int? Limit { get; set; }
        public decimal? Amount { get; set; }
        public double? Percent { get; set; }
        public decimal Total { get; set; }
        public string Description { get; set; } = null!;
        public Guid? DiscountId { get; set; }
    }
}
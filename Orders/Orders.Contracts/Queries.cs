using System;
using System.Collections.Generic;

namespace YourBrand.Orders.Contracts
{
    public class GetOrdersQuery
    {
        public int Skip { get; set; }

        public int Limit { get; set; } = 10;

        public bool IncludeItems { get; set; } = true;

        public bool IncludeDiscounts { get; set; } = true;

        public bool IncludeCharges { get; set; } = true;
    }

    public class GetOrdersQueryResponse
    {
        public IEnumerable<OrderDto> Orders { get; set; } = null!;

        public int Total { get; set; }
    }

    public class GetOrderByIdQuery
    {
        public Guid Id { get; set; }
    }

    public class GetOrderByOrderNoQuery
    {
        public int? OrderNo { get; set; }

        public bool IncludeItems { get; set; } = true;

        public bool IncludeDiscounts { get; set; } = true;

        public bool IncludeCharges { get; set; } = true;
    }

    public class GetOrderStatusQuery
    {
        public string? OrderNo { get; set; }
    }

    public class GetOrderTotalsQuery
    {
        public int OrderNo { get; set; }
    }

    public class GetOrderItemsQuery
    {
        public int OrderNo { get; set; }
    }

    public class GetOrderItemsQueryResponse
    {
        public IEnumerable<OrderItemDto> OrderItems { get; set; } = null!;
    }

    public class GetOrderItemQuery
    {
        public int OrderNo { get; set; }

        public Guid OrderItemId { get; set; }
    }

    public class QueryOrdersByCustomFieldValueQuery
    {
        public string CustomFieldId { get; set; } = null!;

        public string? Value { get; set; }
    }

    public class QueryOrdersByCustomFieldValueQueryResponse
    {
        public IEnumerable<OrderDto> Orders { get; set; } = null!;
    }

    public class GetOrderStatusesQuery
    {
        public int Skip { get; set; }

        public int Limit { get; set; } = 10;

        public bool IncludeItems { get; set; } = true;

        public bool IncludeDiscounts { get; set; } = true;

        public bool IncludeCharges { get; set; } = true;
    }

    public class GetOrderStatusesQueryResponse
    {
        public IEnumerable<OrderStatusDto> OrderStatuses { get; set; } = null!;

        public int Total { get; set; }
    }
}
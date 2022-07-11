using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MassTransit;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using OrderPriceCalculator;

using YourBrand.Orders.Contracts;
using YourBrand.Orders.Domain.Entities;
using YourBrand.Orders.Infrastructure.Persistence;

namespace YourBrand.Orders.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly OrdersContext context;

        public OrdersController(
            ILogger<OrdersController> logger,
            OrdersContext context)
        {
            _logger = logger;
            this.context = context;
        }

        /*

        /// <summary>
        /// Get Orders
        /// </summary>
        [HttpGet]
        public async Task<IEnumerable<OrderDto>> GetOrders(
            [FromServices] IRequestClient<GetOrdersQuery> client)
        {
            var response = await client.GetResponse<GetOrdersQueryResponse>(
                new GetOrdersQuery()
            );

            return response.Message.Orders;
        }

        /// <summary>
        /// Get Order by number
        /// </summary>
        [HttpGet("{orderNo}")]
        public async Task<OrderDto> GetOrder(
            [FromServices] IRequestClient<GetOrderByOrderNoQuery> client,
            int orderNo)
        {
            var response = await client.GetResponse<OrderDto>(
                new GetOrderByOrderNoQuery()
                {
                    OrderNo = orderNo
                }
            );

            return response.Message;
        }
        */

        [HttpGet]
        public async Task<YourBrand.Orders.Models.Orders> GetOrders(int skip = 0, int limit = 10, [FromQuery] string[] embed = null!)
        {
            var query = context.Orders
                .AsQueryable()
                .IncludeAll(includeItems: embed.Contains("items"), includeSubscriptions: false, includeCustomFields: true /* embed.Contains("customFields") */)
                .OrderBy(c => c.Created)
                .AsNoTracking();

            var total = await query
                .CountAsync();

            var orders = await query
                .Skip(skip)
                .Take(limit)
                .ToArrayAsync();

            var r = YourBrand.Orders.Models.Mapper.Map(orders, embed);

            r.Count = orders.Count();
            r.Total = total;

            return YourBrand.Orders.Hypermedia.Mapper.Append("/api/orders", r, skip, limit, embed, (skip + r.Count) < r.Total);
        }

        [HttpGet("{orderNo}")]
        public async Task<YourBrand.Orders.Models.Order> GetOrderByNo(int orderNo, [FromQuery] string[] embed)
        {
            var order = await context.Orders
                .AsQueryable()
                .IncludeAll(includeItems: embed.Contains("items"), includeSubscriptions: false, includeCustomFields: true /* embed.Contains("customFields") */)
                .OrderBy(c => c.Created)
                .AsNoTracking()
                .FirstAsync(x => x.OrderNo == orderNo);

            return YourBrand.Orders.Models.Mapper.Map(order, embed);
        }

        /// <summary>
        /// Get Order totals
        /// </summary>
        [HttpGet("{orderNo}/Totals")]
        public async Task<OrderTotalsDto> GetOrderTotals(
            [FromServices] IRequestClient<GetOrderTotalsQuery> client,
            int orderNo)
        {
            var response = await client.GetResponse<OrderTotalsDto>(new GetOrderTotalsQuery() { OrderNo = orderNo });
            return response.Message;
        }

        /// <summary>
        /// Create Order
        /// </summary>
        [HttpPost]
        public async Task<int> CreateOrder(
            [FromServices] IRequestClient<CreateOrderCommand> client, [FromBody] CreateOrderDto? dto)
        {
            var response = await client.GetResponse<CreateOrderCommandResponse>(new CreateOrderCommand(dto));
            return response.Message.OrderNo;
        }

        /// <summary>
        /// Create Order
        /// </summary>
        [HttpPost("Create")]
        public async Task<int> CreateOrder2(
            [FromServices] IRequestClient<CreateOrderCommand> client)
        {
            var response = await client.GetResponse<CreateOrderCommandResponse>(new CreateOrderCommand(null));
            return response.Message.OrderNo;
        }

        /// <summary>
        /// Place Order
        /// </summary>
        [HttpPost("{orderNo}/Place")]
        public async Task PlaceOrder(
            [FromServices] IRequestClient<PlaceOrderCommand> client, int orderNo)
        {
            var response = await client.GetResponse<PlaceOrderCommandResponse>(new PlaceOrderCommand(orderNo));
        }

        /// <summary>
        /// Update Order Status
        /// </summary>
        [HttpPut("{orderNo}/Status")]
        public async Task UpdateOrderStatus([FromServices] IRequestClient<UpdateOrderStatusCommand> client, int orderNo, string orderStatusId)
        {
            await client.GetResponse<UpdateOrderStatusCommandResponse>(new UpdateOrderStatusCommand(orderNo, orderStatusId));
        }

        /// <summary>
        /// Add Order Item
        /// </summary>
        [HttpPost("{orderNo}/Items")]
        public async Task<OrderItemDto> AddItem([FromServices] IRequestClient<AddOrderItemCommand> client,
            int orderNo,
            string? description,
            string? itemId,
            string? unit,
            double quantity = 1)
        {
            var response = await client.GetResponse<OrderItemDto>(new AddOrderItemCommand(orderNo, description, itemId, unit, quantity));
            return response.Message;
        }

        /// <summary>
        /// Get Order Items
        /// </summary>
        [HttpGet("{orderNo}/Items")]
        public async Task<IEnumerable<OrderItemDto>> GetOrderItems(
            [FromServices] IRequestClient<GetOrderItemsQuery> client,
            int orderNo)
        {
            var response = await client.GetResponse<GetOrderItemsQueryResponse>(new GetOrderItemsQuery() { OrderNo = orderNo });
            return response.Message.OrderItems;
        }

        /// <summary>
        /// Get Order Item by Id
        /// </summary>
        [HttpGet("{orderNo}/Items/{orderItemId}")]
        public async Task<OrderItemDto> GetItem(
            [FromServices] IRequestClient<GetOrderItemQuery> client,
            int orderNo, Guid orderItemId)
        {
            var response = await client.GetResponse<OrderItemDto>(new GetOrderItemQuery() { OrderNo = orderNo, OrderItemId = orderItemId });
            return response.Message;
        }

        /// <summary>
        /// Remove Order Item
        /// </summary>
        [HttpDelete("{orderNo}/Items/{orderItemId}")]
        public async Task RemoveItem(
            [FromServices] IRequestClient<RemoveOrderItemCommand> client,
            int orderNo, Guid orderItemId)
        {
            await client.GetResponse<RemoveOrderItemCommandResponse>(new RemoveOrderItemCommand(orderNo, orderItemId));
        }

        /// <summary>
        /// Update Order Item quantity
        /// </summary>
        [HttpPut("{orderNo}/Items/{orderItemId}/Quantity")]
        public async Task UpdateItemQuantity([FromServices] IRequestClient<UpdateOrderItemQuantityCommand> client, int orderNo, Guid orderItemId, [FromBody] double quantity)
        {
            await client.GetResponse<UpdateOrderItemQuantityCommandResponse>(new UpdateOrderItemQuantityCommand(orderNo, orderItemId, quantity));
        }

        /// <summary>
        /// Remove all Order Items
        /// </summary>
        [HttpDelete("{orderNo}/Items")]
        public async Task ClearOrder([FromServices] IRequestClient<ClearOrderCommand> client, int orderNo)
        {
            await client.GetResponse<ClearOrderCommandResponse>(new ClearOrderCommand(orderNo));
        }

        /// <summary>
        /// Add Discount to Order
        /// </summary>
        [HttpPost("{orderNo}/Discounts")]
        public async Task AddDiscountToOrder([FromServices] IRequestClient<AddDiscountToOrderCommand> client, int orderNo, [FromBody] DiscountDetails details)
        {
            await client.GetResponse<AddDiscountToOrderCommandResponse>(
                new AddDiscountToOrderCommand(orderNo, details)
            );
        }

        /// <summary>
        /// Remove Discount from Order
        /// </summary>
        [HttpDelete("{orderNo}/Discounts/{discountId}")]
        public async Task RemoveDiscountFromItem(
            [FromServices] IRequestClient<RemoveDiscountFromOrderCommand> client,
            int orderNo, Guid discountId)
        {
            await client.GetResponse<RemoveDiscountFromOrderCommandResponse>(new RemoveDiscountFromOrderCommand(orderNo, discountId));
        }

        /// <summary>
        /// Add Discount to Order Item
        /// </summary>
        [HttpPost("{orderNo}/Items/{orderItemId}/Discounts")]
        public async Task AddDiscountToOrderItem([FromServices] IRequestClient<AddDiscountToOrderItemCommand> client, int orderNo, Guid orderItemId, [FromBody] DiscountDetails details)
        {
            await client.GetResponse<AddDiscountToOrderItemCommandResponse>(
                new AddDiscountToOrderItemCommand(orderNo, orderItemId, details)
            );
        }

        /// <summary>
        /// Remove Discount from Order Item
        /// </summary>
        [HttpDelete("{orderNo}/Items/{orderItemId}/Discounts/{discountId}")]
        public async Task RemoveDiscountFromOrderItem(
            [FromServices] IRequestClient<RemoveDiscountFromOrderItemCommand> client,
            int orderNo, Guid orderItemId, Guid discountId)
        {
            await client.GetResponse<RemoveDiscountFromOrderItemCommandResponse>(new RemoveDiscountFromOrderItemCommand(orderNo, orderItemId, discountId));
        }

        /// <summary>
        /// Add Charge to Order
        /// </summary>
        [HttpPost("{orderNo}/Charges")]
        public async Task AddChargeToOrder([FromServices] IRequestClient<AddChargeToOrderCommand> client, int orderNo, [FromBody] ChargeDetails details)
        {
            await client.GetResponse<AddChargeToOrderCommandResponse>(
                new AddChargeToOrderCommand(orderNo, details)
            );
        }

        /// <summary>
        /// Remove Charge from Order
        /// </summary>
        [HttpDelete("{orderNo}/Charges/{chargeId}")]
        public async Task RemoveChargeFromItem(
            [FromServices] IRequestClient<RemoveChargeFromOrderCommand> client,
            int orderNo, Guid chargeId)
        {
            await client.GetResponse<RemoveChargeFromOrderCommandResponse>(new RemoveChargeFromOrderCommand(orderNo, chargeId));
        }

        /// <summary>
        /// Add Charge to Order Item
        /// </summary>
        [HttpPost("{orderNo}/Items/{orderItemId}/Charges")]
        public async Task AddChargeToOrderItem([FromServices] IRequestClient<AddChargeToOrderItemCommand> client, int orderNo, Guid orderItemId, [FromBody] ChargeDetails details)
        {
            await client.GetResponse<AddChargeToOrderItemCommandResponse>(
                new AddChargeToOrderItemCommand(orderNo, orderItemId, details)
            );
        }

        /// <summary>
        /// Remove Charge from Order Item
        /// </summary>
        [HttpDelete("{orderNo}/Items/{orderItemId}/Charges/{chargeId}")]
        public async Task RemoveChargeFromOrderItem(
            [FromServices] IRequestClient<RemoveChargeFromOrderItemCommand> client,
            int orderNo, Guid orderItemId, Guid chargeId)
        {
            await client.GetResponse<RemoveChargeFromOrderItemCommandResponse>(new RemoveChargeFromOrderItemCommand(orderNo, orderItemId, chargeId));
        }

        /// <summary>
        /// Add Custom Field to Order
        /// </summary>
        [HttpPost("{orderNo}/CustomFields")]
        public async Task AddCustomFieldToOrder([FromServices] IRequestClient<AddCustomFieldToOrderCommand> client, int orderNo, [FromBody] CreateCustomFieldDetails details)
        {
            await client.GetResponse<AddCustomFieldToOrderCommandResponse>(
                new AddCustomFieldToOrderCommand(orderNo, details)
            );
        }

        /// <summary>
        /// Remove Custom Field from Order
        /// </summary>
        [HttpDelete("{orderNo}/CustomFields/{customFieldId}")]
        public async Task RemoveCustomFieldFromItem(
            [FromServices] IRequestClient<RemoveCustomFieldFromOrderCommand> client,
            int orderNo, string customFieldId)
        {
            await client.GetResponse<RemoveCustomFieldFromOrderCommandResponse>(new RemoveCustomFieldFromOrderCommand(orderNo, customFieldId));
        }

        /// <summary>
        /// Add Custom Field to Order Item
        /// </summary>
        [HttpPost("{orderNo}/Items/{orderItemId}/CustomFields")]
        public async Task AddCustomFieldToOrderItem([FromServices] IRequestClient<AddCustomFieldToOrderItemCommand> client, int orderNo, Guid orderItemId, [FromBody] CreateCustomFieldDetails details)
        {
            await client.GetResponse<AddCustomFieldToOrderItemCommandResponse>(
                new AddCustomFieldToOrderItemCommand(orderNo, orderItemId, details)
            );
        }

        /// <summary>
        /// Remove Custom Field from Order Item
        /// </summary>
        [HttpDelete("{orderNo}/Items/{orderItemId}/CustomFields/{customFieldId}")]
        public async Task RemoveCustomFieldFromOrderItem(
            [FromServices] IRequestClient<RemoveCustomFieldFromOrderItemCommand> client,
            int orderNo, Guid orderItemId, string customFieldId)
        {
            await client.GetResponse<RemoveCustomFieldFromOrderItemCommandResponse>(new RemoveCustomFieldFromOrderItemCommand(orderNo, orderItemId, customFieldId));
        }

        [HttpGet("QueryOrdersByCustomField")]
        public async Task<IEnumerable<OrderDto>> QueryOrdersByCustomField([FromServices] IRequestClient<QueryOrdersByCustomFieldValueQuery> client, [FromQuery] string customFieldId, [FromQuery] string? value)
        {
            var r = await client.GetResponse<QueryOrdersByCustomFieldValueQueryResponse>(new QueryOrdersByCustomFieldValueQuery()
            {
                CustomFieldId = customFieldId,
                Value = value
            });
            return r.Message.Orders;
        }
    }
}
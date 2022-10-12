using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MassTransit;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using OrderPriceCalculator;

using YourBrand.Orders.Application.Orders;
using YourBrand.Orders.Application.Orders.Commands;
using YourBrand.Orders.Application.Orders.Queries;
using YourBrand.Orders.Contracts;
using YourBrand.Orders.Domain.Entities;
using YourBrand.Orders.Infrastructure.Persistence;

namespace YourBrand.Orders.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly OrdersContext context;
        private readonly IMediator _mediator;

        public OrdersController(
            ILogger<OrdersController> logger,
            OrdersContext context,
            IMediator mediator)
        {
            _logger = logger;
            this.context = context;
            _mediator = mediator;
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
        public async Task<OrderTotalsDto> GetOrderTotals(int orderNo)
        {
            return await _mediator.Send(new GetOrderTotalsQuery() { OrderNo = orderNo });
        }

        /// <summary>
        /// Create Order
        /// </summary>
        [HttpPost]
        public async Task<OrderDto> CreateOrder(
            [FromBody] CreateOrderDto? dto)
        {
            //var response = await client.GetResponse<CreateOrderCommandResponse>(new CreateOrderCommand(dto));
            //return response.Message.OrderNo;

            return await _mediator.Send(new CreateOrderCommand(dto));
        }

        /// <summary>
        /// Create Order
        /// </summary>
        [HttpPost("Create")]
        public async Task<OrderDto> CreateOrder2()
        {
                //var response = await client.GetResponse<CreateOrderCommandResponse>(new CreateOrderCommand(dto));
            //return response.Message.OrderNo;

            return await _mediator.Send(new CreateOrderCommand());
        }

        /// <summary>
        /// Place Order
        /// </summary>
        [HttpPost("{orderNo}/Place")]
        public async Task PlaceOrder(int orderNo)
        {
            await _mediator.Send(new PlaceOrderCommand(orderNo));
        }

        /// <summary>
        /// Update Order Status
        /// </summary>
        [HttpPut("{orderNo}/Status")]
        public async Task UpdateOrderStatus(int orderNo, string orderStatusId)
        {
            await _mediator.Send(new UpdateOrderStatusCommand(orderNo, orderStatusId));
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
            return await _mediator.Send(new AddOrderItemCommand(orderNo, description, itemId, unit, quantity));
        }

        /// <summary>
        /// Get Order Items
        /// </summary>
        [HttpGet("{orderNo}/Items")]
        public async Task<IEnumerable<OrderItemDto>> GetOrderItems(int orderNo)
        {
            return await _mediator.Send(new GetOrderItemsQuery { OrderNo = orderNo });
        }

        /// <summary>
        /// Get Order Item by Id
        /// </summary>
        [HttpGet("{orderNo}/Items/{orderItemId}")]
        public async Task<OrderItemDto> GetItem(int orderNo, Guid orderItemId)
        {
            return await _mediator.Send(new GetOrderItemQuery() { OrderNo = orderNo, OrderItemId = orderItemId });
        }

        /// <summary>
        /// Remove Order Item
        /// </summary>
        [HttpDelete("{orderNo}/Items/{orderItemId}")]
        public async Task RemoveItem(int orderNo, Guid orderItemId)
        {
            await _mediator.Send(new RemoveOrderItemCommand(orderNo, orderItemId));
        }

        /// <summary>
        /// Update Order Item quantity
        /// </summary>
        [HttpPut("{orderNo}/Items/{orderItemId}/Quantity")]
        public async Task UpdateItemQuantity(int orderNo, Guid orderItemId, [FromBody] double quantity)
        {
            await _mediator.Send(new UpdateOrderItemQuantityCommand(orderNo, orderItemId, quantity));
        }

        /// <summary>
        /// Remove all Order Items
        /// </summary>
        [HttpDelete("{orderNo}/Items")]
        public async Task ClearOrder(int orderNo)
        {
            await _mediator.Send(new ClearOrderCommand(orderNo));
        }

        /// <summary>
        /// Add Discount to Order
        /// </summary>
        [HttpPost("{orderNo}/Discounts")]
        public async Task AddDiscountToOrder(int orderNo, [FromBody] DiscountDetails details)
        {
            await _mediator.Send(
                new AddDiscountToOrderCommand(orderNo, details)
            );
        }

        /// <summary>
        /// Remove Discount from Order
        /// </summary>
        [HttpDelete("{orderNo}/Discounts/{discountId}")]
        public async Task RemoveDiscountFromItem(int orderNo, Guid discountId)
        {
            await _mediator.Send(new RemoveDiscountFromOrderCommand(orderNo, discountId));
        }

        /// <summary>
        /// Add Discount to Order Item
        /// </summary>
        [HttpPost("{orderNo}/Items/{orderItemId}/Discounts")]
        public async Task AddDiscountToOrderItem(int orderNo, Guid orderItemId, [FromBody] DiscountDetails details)
        {
            await _mediator.Send(
                new AddDiscountToOrderItemCommand(orderNo, orderItemId, details)
            );
        }

        /// <summary>
        /// Remove Discount from Order Item
        /// </summary>
        [HttpDelete("{orderNo}/Items/{orderItemId}/Discounts/{discountId}")]
        public async Task RemoveDiscountFromOrderItem(int orderNo, Guid orderItemId, Guid discountId)
        {
            await _mediator.Send(new RemoveDiscountFromOrderItemCommand(orderNo, orderItemId, discountId));
        }

        /// <summary>
        /// Add Charge to Order
        /// </summary>
        [HttpPost("{orderNo}/Charges")]
        public async Task AddChargeToOrder(int orderNo, [FromBody] ChargeDetails details)
        {
            await _mediator.Send(new AddChargeToOrderCommand(orderNo, details));
        }

        /// <summary>
        /// Remove Charge from Order
        /// </summary>
        [HttpDelete("{orderNo}/Charges/{chargeId}")]
        public async Task RemoveChargeFromItem(int orderNo, Guid chargeId)
        {
            await _mediator.Send(new RemoveChargeFromOrderCommand(orderNo, chargeId));
        }

        /// <summary>
        /// Add Charge to Order Item
        /// </summary>
        [HttpPost("{orderNo}/Items/{orderItemId}/Charges")]
        public async Task AddChargeToOrderItem(int orderNo, Guid orderItemId, [FromBody] ChargeDetails details)
        {
            await _mediator.Send(new AddChargeToOrderItemCommand(orderNo, orderItemId, details));
        }

        /// <summary>
        /// Remove Charge from Order Item
        /// </summary>
        [HttpDelete("{orderNo}/Items/{orderItemId}/Charges/{chargeId}")]
        public async Task RemoveChargeFromOrderItem(int orderNo, Guid orderItemId, Guid chargeId)
        {
            await _mediator.Send(new RemoveChargeFromOrderItemCommand(orderNo, orderItemId, chargeId));
        }

        /// <summary>
        /// Add Custom Field to Order
        /// </summary>
        [HttpPost("{orderNo}/CustomFields")]
        public async Task AddCustomFieldToOrder(int orderNo, [FromBody] CreateCustomFieldDetails details)
        {
            await _mediator.Send(new AddCustomFieldToOrderCommand(orderNo, details));
        }

        /// <summary>
        /// Remove Custom Field from Order
        /// </summary>
        [HttpDelete("{orderNo}/CustomFields/{customFieldId}")]
        public async Task RemoveCustomFieldFromItem(int orderNo, string customFieldId)
        {
            await _mediator.Send(new RemoveCustomFieldFromOrderCommand(orderNo, customFieldId));
        }

        /// <summary>
        /// Add Custom Field to Order Item
        /// </summary>
        [HttpPost("{orderNo}/Items/{orderItemId}/CustomFields")]
        public async Task AddCustomFieldToOrderItem(int orderNo, Guid orderItemId, [FromBody] CreateCustomFieldDetails details)
        {
            await _mediator.Send(new AddCustomFieldToOrderItemCommand(orderNo, orderItemId, details));
        }

        /// <summary>
        /// Remove Custom Field from Order Item
        /// </summary>
        [HttpDelete("{orderNo}/Items/{orderItemId}/CustomFields/{customFieldId}")]
        public async Task RemoveCustomFieldFromOrderItem(int orderNo, Guid orderItemId, string customFieldId)
        {
            await _mediator.Send(new RemoveCustomFieldFromOrderItemCommand(orderNo, orderItemId, customFieldId));
        }

        [HttpGet("QueryOrdersByCustomField")]
        public async Task<IEnumerable<OrderDto>> QueryOrdersByCustomField([FromQuery] string customFieldId, [FromQuery] string? value)
        {
            var r = await _mediator.Send(new QueryOrdersByCustomFieldValueQuery()
            {
                CustomFieldId = customFieldId,
                Value = value
            });
            return r.Orders;
        }
    }
}
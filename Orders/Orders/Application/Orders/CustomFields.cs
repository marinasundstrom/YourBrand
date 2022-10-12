using System;
using System.Linq;
using System.Threading.Tasks;

using YourBrand.Catalog.Client;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using OrderPriceCalculator;

using YourBrand.Orders.Application.Subscriptions;
using YourBrand.Orders.Contracts;
using YourBrand.Orders.Domain.Entities;
using YourBrand.Orders.Infrastructure.Persistence;

namespace YourBrand.Orders.Application.Orders;

public class CreateCustomFieldDetails
{
    public string CustomFieldId { get; set; } = null!;

    public string Value { get; set; } = null!;
}

public class AddCustomFieldToOrderCommand : IRequest
{
    public AddCustomFieldToOrderCommand(int orderNo, CreateCustomFieldDetails customFieldDetails)
    {
        OrderNo = orderNo;
        CreateCustomFieldDetails = customFieldDetails;
    }

    public int OrderNo { get; }

    public CreateCustomFieldDetails CreateCustomFieldDetails { get; }

    public class AddCustomFieldToOrderCommandHandler : IRequestHandler<AddCustomFieldToOrderCommand>
    {
        private readonly ILogger<AddCustomFieldToOrderCommandHandler> _logger;
        private readonly OrdersContext context;
 
        public AddCustomFieldToOrderCommandHandler(
            ILogger<AddCustomFieldToOrderCommandHandler> logger,
            OrdersContext context)
        {
            _logger = logger;
            this.context = context;
        }

        public async Task<Unit> Handle(AddCustomFieldToOrderCommand request, CancellationToken cancellationToken)
        {
            var message = request;

            var details = message.CreateCustomFieldDetails;

            var order = await context.Orders
                 .IncludeAll()
                 .Where(c => c.OrderNo == message.OrderNo)
                 .FirstOrDefaultAsync();

            if (order is null)
            {
                throw new Exception();
            }

            var customField = new CustomField
            {
                Id = Guid.NewGuid(),
                CustomFieldId = details.CustomFieldId,
                Value = details.Value
            };

            order.CustomFields.Add(customField);

            context.CustomFields.Add(customField);

            order.Update();

            await context.SaveChangesAsync();

            return Unit.Value;
        }
    }
}

public class RemoveCustomFieldFromOrderCommand : IRequest
{
    public RemoveCustomFieldFromOrderCommand(
        int orderNo,
        string customFieldId
    )
    {
        OrderNo = orderNo;
        CustomFieldId = customFieldId;
    }

    public int OrderNo { get; }

    public string CustomFieldId { get; }

    public class RemoveCustomFieldFromOrderCommandHandler : IRequestHandler<RemoveCustomFieldFromOrderCommand>
    {
        private readonly ILogger<RemoveCustomFieldFromOrderCommandHandler> _logger;
        private readonly OrdersContext context;
 
        public RemoveCustomFieldFromOrderCommandHandler(
            ILogger<RemoveCustomFieldFromOrderCommandHandler> logger,
            OrdersContext context)
        {
            _logger = logger;
            this.context = context;
        }

        public async Task<Unit> Handle(RemoveCustomFieldFromOrderCommand request, CancellationToken cancellationToken)
        {
            var message = request;

            var order = await context.Orders
                .IncludeAll()
                .Where(c => c.OrderNo == message.OrderNo)
                .FirstOrDefaultAsync();

            if (order is null)
            {
                throw new Exception();
            }

            var customField = order.CustomFields.FirstOrDefault(x => x.CustomFieldId == message.CustomFieldId);

            if (customField is null)
            {
                throw new Exception();
            }

            order.CustomFields.Remove(customField);

            context.CustomFields.Remove(customField);

            order.Update();

            await context.SaveChangesAsync();

            return Unit.Value;
        }
    }
}

public class AddCustomFieldToOrderItemCommand : IRequest
{
    public AddCustomFieldToOrderItemCommand(int orderNo, Guid orderItemId, CreateCustomFieldDetails customFieldDetails)
    {
        OrderNo = orderNo;
        OrderItemId = orderItemId;
        CreateCustomFieldDetails = customFieldDetails;
    }

    public int OrderNo { get; }

    public Guid OrderItemId { get; }

    public CreateCustomFieldDetails CreateCustomFieldDetails { get; }

    public class AddCustomFieldToOrderItemCommandHandler : IRequestHandler<AddCustomFieldToOrderItemCommand>
    {
        private readonly ILogger<AddCustomFieldToOrderItemCommandHandler> _logger;
        private readonly OrdersContext context;
 
        public AddCustomFieldToOrderItemCommandHandler(
            ILogger<AddCustomFieldToOrderItemCommandHandler> logger,
            OrdersContext context)
        {
            _logger = logger;
            this.context = context;
        }

        public async Task<Unit> Handle(AddCustomFieldToOrderItemCommand request, CancellationToken cancellationToken)
        {
            var message = request;

            var details = message.CreateCustomFieldDetails;

            var order = await context.Orders
                            .IncludeAll()
                            .Where(c => c.OrderNo == message.OrderNo)
                            .FirstOrDefaultAsync();

            if (order is null)
            {
                throw new Exception();
            }

            var orderItem = order.Items.FirstOrDefault(i => i.Id == message.OrderItemId);

            if (orderItem is null)
            {
                throw new Exception();
            }

            var customField = new CustomField
            {
                Id = Guid.NewGuid(),
                CustomFieldId = details.CustomFieldId,
                Value = details.Value
            };

            orderItem.CustomFields.Add(customField);

            context.CustomFields.Add(customField);

            order.Update();

            await context.SaveChangesAsync();

            return Unit.Value;
        }
    }
}

public class RemoveCustomFieldFromOrderItemCommand : IRequest
{
    public RemoveCustomFieldFromOrderItemCommand(
        int orderNo,
        Guid orderItemId,
        string customFieldId
    )
    {
        OrderNo = orderNo;
        OrderItemId = orderItemId;
        CustomFieldId = customFieldId;
    }

    public int OrderNo { get; }

    public Guid OrderItemId { get; }

    public string CustomFieldId { get; }

    public class RemoveCustomFieldFromOrderItemCommandHandler : IRequestHandler<RemoveCustomFieldFromOrderItemCommand>
    {
        private readonly ILogger<RemoveCustomFieldFromOrderItemCommandHandler> _logger;
        private readonly OrdersContext context;
 
        public RemoveCustomFieldFromOrderItemCommandHandler(
            ILogger<RemoveCustomFieldFromOrderItemCommandHandler> logger,
            OrdersContext context)
        {
            _logger = logger;
            this.context = context;
        }

        public async Task<Unit> Handle(RemoveCustomFieldFromOrderItemCommand request, CancellationToken cancellationToken)
        {
            var message = request;

            var order = await context.Orders
             .IncludeAll()
             .Where(c => c.OrderNo == message.OrderNo)
             .FirstOrDefaultAsync();

            if (order is null)
            {
                throw new Exception();
            }

            var orderItem = order.Items.FirstOrDefault(i => i.Id == message.OrderItemId);

            if (orderItem is null)
            {
                throw new Exception();
            }

            var customField = orderItem.CustomFields.FirstOrDefault(x => x.CustomFieldId == message.CustomFieldId);

            if (customField is null)
            {
                throw new Exception();
            }

            orderItem.CustomFields.Remove(customField);

            context.CustomFields.Remove(customField);

            order.Update();

            await context.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
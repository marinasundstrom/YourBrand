using System;
using System.Linq;
using System.Threading.Tasks;

using YourBrand.Catalog.Client;

using MassTransit;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using OrderPriceCalculator;

using YourBrand.Orders.Application.Subscriptions;
using YourBrand.Orders.Contracts;
using YourBrand.Orders.Domain.Entities;
using YourBrand.Orders.Infrastructure.Persistence;

namespace YourBrand.Orders.Application.Orders;

public class AddCustomFieldToOrderCommandHandler : IConsumer<AddCustomFieldToOrderCommand>
{
    private readonly ILogger<AddCustomFieldToOrderCommandHandler> _logger;
    private readonly OrdersContext context;
    private readonly IBus bus;

    public AddCustomFieldToOrderCommandHandler(
        ILogger<AddCustomFieldToOrderCommandHandler> logger,
        OrdersContext context,
        IBus bus)
    {
        _logger = logger;
        this.context = context;
        this.bus = bus;
    }

    public async Task Consume(ConsumeContext<AddCustomFieldToOrderCommand> consumeContext)
    {
        var message = consumeContext.Message;

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

        await consumeContext.RespondAsync<AddCustomFieldToOrderCommandResponse>(new AddCustomFieldToOrderCommandResponse());
    }
}

public class RemoveCustomFieldFromOrderCommandHandler : IConsumer<RemoveCustomFieldFromOrderCommand>
{
    private readonly ILogger<RemoveCustomFieldFromOrderCommandHandler> _logger;
    private readonly OrdersContext context;
    private readonly IBus bus;

    public RemoveCustomFieldFromOrderCommandHandler(
        ILogger<RemoveCustomFieldFromOrderCommandHandler> logger,
        OrdersContext context,
        IBus bus)
    {
        _logger = logger;
        this.context = context;
        this.bus = bus;
    }

    public async Task Consume(ConsumeContext<RemoveCustomFieldFromOrderCommand> consumeContext)
    {
        var message = consumeContext.Message;

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

        await consumeContext.RespondAsync<RemoveCustomFieldFromOrderCommandResponse>(new RemoveCustomFieldFromOrderCommandResponse());
    }
}

public class AddCustomFieldToOrderItemCommandHandler : IConsumer<AddCustomFieldToOrderItemCommand>
{
    private readonly ILogger<AddCustomFieldToOrderItemCommandHandler> _logger;
    private readonly OrdersContext context;
    private readonly IBus bus;

    public AddCustomFieldToOrderItemCommandHandler(
        ILogger<AddCustomFieldToOrderItemCommandHandler> logger,
        OrdersContext context,
        IBus bus)
    {
        _logger = logger;
        this.context = context;
        this.bus = bus;
    }

    public async Task Consume(ConsumeContext<AddCustomFieldToOrderItemCommand> consumeContext)
    {
        var message = consumeContext.Message;

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

        await consumeContext.RespondAsync<AddCustomFieldToOrderItemCommandResponse>(new AddCustomFieldToOrderItemCommandResponse());
    }
}

public class RemoveCustomFieldFromOrderItemCommandHandler : IConsumer<RemoveCustomFieldFromOrderItemCommand>
{
    private readonly ILogger<RemoveCustomFieldFromOrderItemCommandHandler> _logger;
    private readonly OrdersContext context;
    private readonly IBus bus;

    public RemoveCustomFieldFromOrderItemCommandHandler(
        ILogger<RemoveCustomFieldFromOrderItemCommandHandler> logger,
        OrdersContext context,
        IBus bus)
    {
        _logger = logger;
        this.context = context;
        this.bus = bus;
    }

    public async Task Consume(ConsumeContext<RemoveCustomFieldFromOrderItemCommand> consumeContext)
    {
        var message = consumeContext.Message;

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

        await consumeContext.RespondAsync<RemoveCustomFieldFromOrderItemCommandResponse>(new RemoveCustomFieldFromOrderItemCommandResponse());
    }
}
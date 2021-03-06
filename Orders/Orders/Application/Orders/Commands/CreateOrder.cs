using YourBrand.Catalog.Client;

using MassTransit;

using Microsoft.EntityFrameworkCore;

using OrderPriceCalculator;
using YourBrand.Orders.Contracts;
using YourBrand.Orders.Domain.Entities;
using YourBrand.Orders.Infrastructure.Persistence;
using YourBrand.Orders.Domain.Events;

namespace YourBrand.Orders.Application.Orders.Commands;

public class CreateOrderCommandHandler : IConsumer<CreateOrderCommand>
{
    private readonly ILogger<CreateOrderCommandHandler> _logger;
    private readonly IProductsClient productsClient;
    private readonly OrdersContext context;
    private readonly IBus bus;

    public CreateOrderCommandHandler(
        ILogger<CreateOrderCommandHandler> logger,
        IProductsClient productsClient,
        OrdersContext context,
        IBus bus)
    {
        _logger = logger;
        this.productsClient = productsClient;
        this.context = context;
        this.bus = bus;
    }

    public async Task Consume(ConsumeContext<CreateOrderCommand> consumeContext)
    {
        var message = consumeContext.Message;

        var order = new Order();
        order.Id = Guid.NewGuid();
        order.OrderDate = DateTime.Now;

        order.StatusId = message?.Dto?.Status ?? "draft";
        order.StatusDate = DateTime.Now;

        try
        {
            order.OrderNo = await context.Orders.MaxAsync(r => r.OrderNo) + 1;
        }
        catch
        {
            order.OrderNo = 1;
        }

        if (message?.Dto != null)
        {
            var dto = message.Dto;

            foreach (var item in dto.Items)
            {
                await AddItem(order, item);
            }

            if (dto.Charges is not null)
            {
                foreach (var item in dto.Charges)
                {
                    AddCharge(order, item);
                }
            }

            if (dto.Discounts is not null)
            {
                foreach (var item in dto.Discounts)
                {
                    AddDiscount(order, item);
                }
            }

            AddCustomFields(order, dto);
        }

        context.Orders.Add(order);

        order.Update();

        await context.SaveChangesAsync();

        await bus.Publish(new OrderCreatedEvent(order.OrderNo));

        if (order.StatusId == "placed")
        {
            await bus.Publish(new OrderCreatedEvent(order.OrderNo));
        }

        await consumeContext.RespondAsync<CreateOrderCommandResponse>(new CreateOrderCommandResponse(order.OrderNo));
    }

    private void AddCustomFields(Order order, CreateOrderDto dto)
    {
        if (dto.CustomFields is not null)
        {
            foreach (KeyValuePair<string, string> i in dto.CustomFields)
            {
                order.CustomFields.Add(new CustomField()
                {
                    Id = Guid.NewGuid(),
                    CustomFieldId = i.Key,
                    Value = i.Value.ToString()
                });
            }
        }
    }

    private void AddCustomFields(OrderItem orderItem, CreateOrderItemDto dto)
    {
        if (dto.CustomFields is not null)
        {
            foreach (KeyValuePair<string, string> i in dto.CustomFields)
            {
                orderItem.CustomFields.Add(new CustomField()
                {
                    Id = Guid.NewGuid(),
                    CustomFieldId = i.Key,
                    Value = i.Value.ToString()
                });
            }
        }
    }

    private void AddCharge(Order order, OrderChargeDto chargeDto)
    {
        var charge = new OrderCharge()
        {
            Id = Guid.NewGuid(),
            Order = order,
            Quantity = chargeDto.Quantity,
            Limit = chargeDto.Limit,
            Amount = chargeDto.Amount,
            Percent = chargeDto.Percent,
            Description = chargeDto.Description ?? string.Empty
            //ChargeId = chargeDto.ChargeId
        };

        order.Charges.Add(charge);
        context.OrderCharges.Add(charge);
    }

    private void AddCharge(OrderItem orderItem, OrderChargeDto chargeDto)
    {
        var charge = new OrderCharge()
        {
            Id = Guid.NewGuid(),
            Quantity = chargeDto.Quantity,
            Limit = chargeDto.Limit,
            Amount = chargeDto.Amount,
            Percent = chargeDto.Percent,
            Description = chargeDto.Description!
            //ChargeId = chargeDto.ChargeId
        };

        orderItem.Charges.Add(charge);
    }

    private void AddDiscount(Order order, OrderDiscountDto discountDto)
    {
        var discount = new OrderDiscount()
        {
            Id = Guid.NewGuid(),
            Quantity = discountDto.Quantity,
            Limit = discountDto.Limit,
            Amount = discountDto.Amount,
            Percent = discountDto.Percent,
            Description = discountDto.Description!,
            DiscountId = discountDto.DiscountId
        };

        order.Discounts.Add(discount);
    }

    private void AddDiscount(OrderItem orderItem, OrderDiscountDto discountDto)
    {
        var discount = new OrderDiscount()
        {
            Id = Guid.NewGuid(),
            Quantity = discountDto.Quantity,
            Limit = discountDto.Limit,
            Amount = discountDto.Amount,
            Percent = discountDto.Percent,
            Description = discountDto.Description!,
            DiscountId = discountDto.DiscountId
        };

        orderItem.Discounts.Add(discount);
    }

    private async Task AddItem(Order order, CreateOrderItemDto dto)
    {
        ProductDto? product = null;

        if (dto.ItemId is not null)
        {
            product = await productsClient.GetProductAsync(dto.ItemId);
        }

        var orderItem = new OrderItem()
        {
            Id = Guid.NewGuid(),
            ItemId = dto.ItemId!,
            Description = dto.Description ?? product!.Name,
            //Unit = product!.Unit.Name,
            Quantity = dto.Quantity,
            Price = product.Price.GetValueOrDefault(), //Price = product!.VatIncluded ? product.Price : product.Price.AddVat(product.VatRate),
            VatRate = 0.25 //product!.VatRate
        };

        if (dto.Charges is not null)
        {
            foreach (var d in dto.Charges)
            {
                AddCharge(orderItem, d);
            }
        }

        if (dto.Discounts is not null)
        {
            foreach (var d in dto.Discounts)
            {
                AddDiscount(orderItem, d);
            }
        }

        order.Items.Add(orderItem);

        AddCustomFields(orderItem, dto);

        orderItem.Update();

        order.Update();
    }
}

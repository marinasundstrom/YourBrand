using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Domain.Infrastructure;
using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Domain.Entities.Builders;
using YourBrand.Sales.Domain.Events;
using YourBrand.Sales.Domain.ValueObjects;
using YourBrand.Sales.Features.OrderManagement.Orders.Dtos;
using YourBrand.Sales.Features.OrderManagement.Repositories;

namespace YourBrand.Sales.Features.OrderManagement.Orders.Commands;

public sealed record CreateOrder(string OrganizationId, int? Status, SetCustomerDto? Customer, BillingDetailsDto BillingDetails, ShippingDetailsDto? ShippingDetails, IEnumerable<CreateOrderItemDto> Items) : IRequest<Result<OrderDto>>
{
    public sealed class Validator : AbstractValidator<CreateOrder>
    {
        public Validator()
        {
            //RuleFor(x => x.Title).NotEmpty().MaximumLength(60);

            //RuleFor(x => x.Description).MaximumLength(240);
        }
    }

    public sealed class Handler(OrderNumberFetcher orderNumberFetcher, ISalesContext salesContext, TimeProvider timeProvider, IOrderRepository orderRepository, IUnitOfWork unitOfWork, IDomainEventDispatcher domainEventDispatcher) : IRequestHandler<CreateOrder, Result<OrderDto>>
    {
        public async Task<Result<OrderDto>> Handle(CreateOrder request, CancellationToken cancellationToken)
        {
            var orderBuilder = OrderBuilder.NewOrder(request.OrganizationId, typeId: 1, "SEK", true)
                .WithCustomer(new Customer
                {
                    Id = request.Customer!.Id,
                    CustomerNo = 0,
                    Name = request.Customer.Name
                })
                .WithInitialStatus(request.Status ?? (int)OrderStatusEnum.Draft);

            if (request.BillingDetails is not null)
            {
                var billingDetails = new BillingDetails
                {
                    FirstName = request.BillingDetails.FirstName,
                    LastName = request.BillingDetails.LastName,
                    SSN = request.BillingDetails.SSN,
                    Email = request.BillingDetails.Email,
                    PhoneNumber = request.BillingDetails.PhoneNumber,
                    Address = request.BillingDetails.Address.ToAddress()
                };

                orderBuilder.WithBillingDetails(billingDetails);
            }

            if (request.ShippingDetails is not null)
            {
                var shippingDetails = new ShippingDetails
                {
                    FirstName = request.ShippingDetails.FirstName,
                    LastName = request.ShippingDetails.LastName,
                    Address = request.ShippingDetails.Address.ToAddress()
                };

                orderBuilder.WithShippingDetails(shippingDetails);
            }

            var order = orderBuilder.Build();

            await order.AssignOrderNo(orderNumberFetcher, cancellationToken);

            foreach (var orderItem in request.Items)
            {
                var item = order.AddItem(
                    orderItem.Description,
                    orderItem.ItemId,
                    orderItem.UnitPrice,
                    orderItem.RegularPrice,
                    null,
                    null,
                    orderItem.Quantity,
                    orderItem.Unit,
                    orderItem.VatRate,
                    orderItem.Notes,
                    timeProvider);

                if (orderItem.Options is not null)
                {
                    foreach (var option in orderItem.Options) 
                    {
                        item.AddOption(option.Name, option.Description, option.Value, option.ProductId, option.ItemId, option.Price, null, timeProvider);
                    }
                }
                
                if(orderItem.Discounts is not null) 
                {
                    foreach (var discount in orderItem.Discounts)
                    {
                        item.AddPromotionalDiscount(discount.Description, discount.Amount, discount.Rate, timeProvider);
                    }
                }
            }

            order.Update(timeProvider);

            orderRepository.Add(order);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            /*
            if (request.AssigneeId is not null)
            {
                order.UpdateAssigneeId(request.AssigneeId);

                await unitOfWork.SaveChangesAsync(cancellationToken);

                order.ClearDomainEvents();
            }
            */

            await domainEventDispatcher.Dispatch(new OrderCreated(order.Id), cancellationToken);

            order = await orderRepository.GetAll()
                .Include(i => i.CreatedBy)
                .Include(i => i.LastModifiedBy)
                .FirstAsync(x => x.Id == order.Id, cancellationToken);

            return order!.ToDto();
        }
    }
}

public record SetCustomerDto(string Id, string Name);
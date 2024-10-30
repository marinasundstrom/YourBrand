using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Domain.Entities.Builders;
using YourBrand.Sales.Domain.ValueObjects;
using YourBrand.Sales.Features.OrderManagement.Orders;
using YourBrand.Sales.Features.OrderManagement.Orders.Commands;
using YourBrand.Sales.Features.OrderManagement.Orders.Dtos;
using YourBrand.Sales.Persistence;
using YourBrand.Sales.Persistence.Repositories.Mocks;

namespace YourBrand.Sales.Features.SubscriptionManagement;

public record CreateSubscriptionOrder(string OrganizationId, string ProductId, string ProductName, decimal Price, decimal? OriginalPrice, Guid SubscriptionPlanId, DateOnly StartDate, TimeOnly? StartTime, OrderManagement.Orders.Commands.SetCustomerDto Customer, BillingDetailsDto BillingDetails, ShippingDetailsDto? ShippingDetails, string? Notes) : IRequest<OrderDto>
{
    public class Handler(SalesContext salesContext, TimeProvider timeProvider, OrderNumberFetcher orderNumberFetcher, SubscriptionNumberFetcher subscriptionNumberFetcher, SubscriptionOrderGenerator subscriptionOrderGenerator) : IRequestHandler<CreateSubscriptionOrder, OrderDto>
    {
        public async Task<OrderDto> Handle(CreateSubscriptionOrder request, CancellationToken cancellationToken)
        {
            var subscriptionPlan = await salesContext.SubscriptionPlans.FirstOrDefaultAsync(x => x.Id == request.SubscriptionPlanId);

            var subscription = new Subscription()
            {
                Plan = subscriptionPlan!,
                Schedule = subscriptionPlan!.Schedule.Clone(),
                StartDate = request.StartDate,
                //StartTime = request.StartTime, // REVISIT
                EndDate = request.StartDate.AddMonths(12),
                OrganizationId = request.OrganizationId
            };

            subscription.SubscriptionNo = await subscriptionNumberFetcher.GetNextNumberAsync(request.OrganizationId, cancellationToken);

            var orderBuilder = OrderBuilder.NewOrder(request.OrganizationId, typeId: 2, "SEK", true)
                .WithCustomer(new Customer
                {
                    Id = request.Customer!.Id,
                    CustomerNo = 0,
                    Name = request.Customer.Name
                })
                .WithInitialStatus((int)OrderStatusEnum.PendingConfirmation)
                .WithSubscription(subscription);

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

            var orderItem = order.AddItem(
                "Foo", request.ProductId, request.Price, request.OriginalPrice, null, null, 1, null, 0.25, request.Notes, 
                timeProvider);
         
            orderItem.Subscription = subscription;
            orderItem.SubscriptionPlan = subscription.Plan;

            salesContext.Orders.Add(order);

            await salesContext.SaveChangesAsync();

            subscription.Order = order;

            await salesContext.SaveChangesAsync();

            order = await salesContext.Orders
                .IncludeAll()
                .FirstAsync(x => x.Id == order.Id, cancellationToken);

            return order.ToDto();
        }
    }
}
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Domain.ValueObjects;
using YourBrand.Sales.Features.OrderManagement.Orders;
using YourBrand.Sales.Features.OrderManagement.Orders.Dtos;
using YourBrand.Sales.Persistence;
using YourBrand.Sales.Persistence.Repositories.Mocks;

namespace YourBrand.Sales.Features.Subscriptions;

public record CreateSubscription(string OrganizationId, string ProductId, Guid SubscriptionPlanId, DateOnly StartDate, TimeOnly? StartTime, OrderManagement.Orders.Commands.SetCustomerDto Customer, BillingDetailsDto BillingDetails, ShippingDetailsDto? ShippingDetails, string? Notes) : IRequest<OrderDto>
{
    public class Handler(SalesContext salesContext, SubscriptionOrderGenerator subscriptionOrderGenerator) : IRequestHandler<CreateSubscription, OrderDto>
    {
        public async Task<OrderDto> Handle(CreateSubscription request, CancellationToken cancellationToken)
        {
            var subscriptionPlan = await salesContext.SubscriptionPlans.FirstOrDefaultAsync(x => x.Id == request.SubscriptionPlanId);

            var subscription = new Subscription()
            {
                SubscriptionPlan = subscriptionPlan!,
                StartDate = request.StartDate,
                StartTime = request.StartTime,
                OrganizationId = request.OrganizationId
            };

            var order = new Order()
            {
                OrganizationId = request.OrganizationId,
                Customer = new Customer
                {
                    Id = request.Customer.Id,
                    CustomerNo = 0,
                    Name = request.Customer.Name
                }
            };

            order.BillingDetails = request.BillingDetails is null ? null : new BillingDetails
            {
                FirstName = request.BillingDetails.FirstName,
                LastName = request.BillingDetails.LastName,
                SSN = request.BillingDetails.SSN,
                Email = request.BillingDetails.Email,
                PhoneNumber = request.BillingDetails.PhoneNumber,
                Address = Map(request.BillingDetails.Address)
            };

            if (request.ShippingDetails is not null)
            {
                order.ShippingDetails = new ShippingDetails
                {
                    FirstName = request.ShippingDetails.FirstName,
                    LastName = request.ShippingDetails.LastName,
                    CareOf = request.ShippingDetails.CareOf,
                    Address = Map(request.ShippingDetails.Address),
                };
            }

            try
            {
                order.OrderNo = (await salesContext.Orders.MaxAsync(x => x.OrderNo)) + 1;
            }
            catch (InvalidOperationException e)
            {
                order.OrderNo = 1; // Order start number
            }

            var orderItem = order.AddItem("Foo", request.ProductId, 20, null, null, null, 1, null, 0.25, request.Notes);
            orderItem.Subscription = subscription;
            orderItem.SubscriptionPlan = subscription.SubscriptionPlan;

            salesContext.Orders.Add(order);

            await salesContext.SaveChangesAsync();

            order = await salesContext.Orders
                .IncludeAll()
                .FirstAsync(x => x.Id == order.Id, cancellationToken);

            return order.ToDto();
        }

        private Sales.Domain.ValueObjects.Address Map(AddressDto address)
        {
            return new Sales.Domain.ValueObjects.Address()
            {
                Thoroughfare = address.Thoroughfare,
                Premises = address.Premises,
                SubPremises = address.SubPremises,
                PostalCode = address.PostalCode,
                Locality = address.Locality,
                SubAdministrativeArea = address.SubAdministrativeArea,
                AdministrativeArea = address.AdministrativeArea,
                Country = address.Country
            };
        }
    }
}
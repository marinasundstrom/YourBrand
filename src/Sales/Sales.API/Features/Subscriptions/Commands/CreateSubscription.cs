using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Domain.ValueObjects;
using YourBrand.Sales.Features.OrderManagement.Orders.Dtos;
using YourBrand.Sales.Persistence;
using YourBrand.Sales.Domain.Entities;

using static YourBrand.Sales.Features.Subscriptions.Mappings;

namespace YourBrand.Sales.Features.Subscriptions;

public record CreateSubscription(string ProductId, Guid SubscriptionPlanId, string CustomerId, BillingDetailsDto BillingDetails, ShippingDetailsDto? ShippingDetails, string? Notes) : IRequest
{
    public class Handler(SalesContext salesContext, SubscriptionOrderGenerator subscriptionOrderGenerator) : IRequestHandler<CreateSubscription>
    {
        public async Task Handle(CreateSubscription request, CancellationToken cancellationToken)
        {
            var subscriptionPlan = await  salesContext.SubscriptionPlans.FirstOrDefaultAsync(x => x.Id == request.SubscriptionPlanId);

            var subscription = new Subscription() 
            {
                SubscriptionPlan = subscriptionPlan!,
                
            };

            var order = new Order()
            {
                Customer = new Customer {
                    Id = request.CustomerId
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

            await salesContext.SaveChangesAsync();
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
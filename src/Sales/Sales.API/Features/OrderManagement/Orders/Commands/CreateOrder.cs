﻿using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Domain.Infrastructure;
using YourBrand.Sales.Domain.Entities;
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
            var order = Order.Create(organizationId: request.OrganizationId);

            await order.AssignOrderNo(orderNumberFetcher, cancellationToken);

            const int OrderStatusDraft = 1;

            order.UpdateStatus(request.Status ?? OrderStatusDraft, timeProvider);

            if (request.Customer is not null)
            {
                if (order.Customer is null)
                {
                    order.Customer = new Domain.Entities.Customer();
                }

                order.Customer.Id = request.Customer.Id;
                order.Customer.Name = request.Customer.Name;
            }

            order.VatIncluded = true;

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

            foreach (var orderItem in request.Items)
            {
                order.AddItem(orderItem.Description, orderItem.ItemId, orderItem.UnitPrice, orderItem.RegularPrice, null, null, orderItem.Quantity, orderItem.Unit, orderItem.VatRate, orderItem.Notes);
            }

            order.Update();

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

        private Domain.ValueObjects.Address Map(AddressDto address)
        {
            return new Domain.ValueObjects.Address
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

public record SetCustomerDto(string Id, string Name);
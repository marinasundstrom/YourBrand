using Azure.Core;

using MediatR;

using Microsoft.AspNetCore.Http.HttpResults;

using YourBrand.Sales.Domain.ValueObjects;
using YourBrand.Sales.Features.OrderManagement.Orders.Commands;
using YourBrand.Sales.Features.OrderManagement.Orders.Dtos;
using YourBrand.Sales.Features.SubscriptionManagement.Plans;
using YourBrand.Sales.Models;

namespace YourBrand.Sales.Features.SubscriptionManagement;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapSubscriptionEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapSubscriptionPlanEndpoints();

        var versionedApi = app.NewVersionedApi("Subscriptions");

        var group = versionedApi.MapGroup("/v{version:apiVersion}/subscriptions")
            .WithTags("Subscriptions")
            .HasApiVersion(ApiVersions.V1)
            .RequireAuthorization()
            .WithOpenApi();

        group.MapGet("/", GetSubscriptions)
            .WithName($"Subscriptions_{nameof(GetSubscriptions)}");

        group.MapGet("/{id}", GetSubscriptionById)
            .WithName($"Subscriptions_{nameof(GetSubscriptionById)}");

        group.MapGet("/getByNo/{subscriptionNo}", GetSubscriptionByNo)
            .WithName($"Subscriptions_{nameof(GetSubscriptionByNo)}");

        group.MapPost("/", CreateSubscriptionOrder)
            .WithName($"Subscriptions_{nameof(CreateSubscriptionOrder)}");

        group.MapPut("/{id}/status", UpdateSubscriptionStatus)
            .WithName($"Subscriptions_{nameof(UpdateSubscriptionStatus)}");

        return app;
    }

    private static async Task<PagedResult<SubscriptionDto>> GetSubscriptions(string organizationId, int[]? types, int[]? status, string? customerId = null, int page = 1, int pageSize = 10, string? sortBy = null, SortDirection? sortDirection = null, IMediator mediator = default!, CancellationToken cancellationToken = default!)
    {
        return await mediator.Send(new GetSubscriptionsQuery(organizationId, types, status, customerId, page, pageSize, sortBy, sortDirection), cancellationToken);
    }

    private static async Task<SubscriptionDto> GetSubscriptionById(Guid id, IMediator mediator, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetSubscriptionQuery(id), cancellationToken);
    }

    private static async Task<SubscriptionDto> GetSubscriptionByNo(string organizationId, int subscriptionNo, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetSubscriptionByNo(organizationId, subscriptionNo), cancellationToken);

        return result.GetValue();
    }

    private static async Task<OrderDto> CreateSubscriptionOrder(string organizationId, CreateSubscriptionRequest request, IMediator mediator, CancellationToken cancellationToken)
    {
        return await mediator.Send(new CreateSubscriptionOrder(
            organizationId,
            request.ProductId, request.ProductName, request.Price, request.RegularPrice, request.SubscriptionPlanId, request.StartDate, request.StartTime,
            request.EndDate, request.EndTime, 
            request.Customer, request.BillingDetails, request.ShippingDetails, request.Notes
        ), cancellationToken);
    }

    private static async Task UpdateSubscriptionStatus(string organizationId, Guid id, UpdateSubscriptionStatusRequest request, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdateSubscriptionStatus(organizationId, id, request.StatusId), cancellationToken);
    }

}

public sealed record CreateSubscriptionRequest(
    string ProductId, string ProductName, decimal Price, decimal? RegularPrice, Guid SubscriptionPlanId, DateOnly StartDate, TimeOnly? StartTime, DateOnly EndDate, TimeOnly? EndTime, SetCustomerDto Customer,
    BillingDetailsDto BillingDetails, ShippingDetailsDto ShippingDetails, string Notes);

public sealed record UpdateSubscriptionStatusRequest(int StatusId);
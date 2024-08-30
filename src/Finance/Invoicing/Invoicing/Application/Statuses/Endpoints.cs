using MediatR;

using Microsoft.AspNetCore.Http.HttpResults;

using YourBrand.Invoicing;
using YourBrand.Invoicing.Application;
using YourBrand.Sales.Features.InvoiceManagement.Invoices.Statuses.Commands;
using YourBrand.Sales.Features.InvoiceManagement.Invoices.Statuses.Queries;

namespace YourBrand.Sales.Features.InvoiceManagement.Invoices.Statuses;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapInvoiceStatusEndpoints(this IEndpointRouteBuilder app)
    {
        string GetInvoicesExpire20 = nameof(GetInvoicesExpire20);

        var versionedApi = app.NewVersionedApi("InvoiceStatuses");

        var group = versionedApi.MapGroup("/v{version:apiVersion}/invoices/statuses")
            .WithTags("InvoiceStatuses")
            .HasApiVersion(ApiVersions.V1)
            .WithOpenApi();

        group.MapGet("/", GetInvoiceStatuses)
            .WithName($"InvoiceStatuses_{nameof(GetInvoiceStatuses)}");

        group.MapGet("/{id}", GetInvoiceStatusById)
            .WithName($"InvoiceStatuses_{nameof(GetInvoiceStatusById)}");

        group.MapPost("/", CreateInvoiceStatus)
            .WithName($"InvoiceStatuses_{nameof(CreateInvoiceStatus)}");

        group.MapPost("{id}", UpdateInvoiceStatus)
            .WithName($"InvoiceStatuses_{nameof(UpdateInvoiceStatus)}");

        group.MapDelete("{id}", DeleteInvoiceStatus)
            .WithName($"InvoiceStatuses_{nameof(DeleteInvoiceStatus)}");

        return app;
    }

    private static async Task<Ok<ItemsResult<InvoiceStatusDto>>> GetInvoiceStatuses(string organizationId, int page = 1, int pageSize = 10, string? searchTerm = null, string? sortBy = null, SortDirection? sortDirection = null, IMediator mediator = default!, CancellationToken cancellationToken = default!)
    {
        var result = await mediator.Send(new GetInvoiceStatusesQuery(organizationId, page, pageSize, searchTerm, sortBy, sortDirection), cancellationToken);
        return TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<InvoiceStatusDto>, NotFound>> GetInvoiceStatusById(string organizationId, int id, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetInvoiceStatusQuery(organizationId, id), cancellationToken);

        /*
        if (result.HasError(Errors.Invoices.InvoiceNotFound))
        {
            return TypedResults.NotFound();
        }
        */

        return TypedResults.Ok(result);
    }

    private static async Task<Results<Created<InvoiceStatusDto>, NotFound>> CreateInvoiceStatus(string organizationId, CreateInvoiceStatusDto request, IMediator mediator, LinkGenerator linkGenerator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateInvoiceStatusCommand(organizationId, request.Name, request.Handle, request.Description), cancellationToken);

        /*
        if (result.HasError(Errors.Invoices.InvoiceNotFound))
        {
            return TypedResults.NotFound();
        }

        var invoice = result.GetValue();*/

        var path = linkGenerator.GetPathByName(nameof(GetInvoiceStatusById), new { id = result.Id });

        return TypedResults.Created(path, result);
    }

    private static async Task<Results<Ok, NotFound>> UpdateInvoiceStatus(string organizationId, int id, UpdateInvoiceStatusDto request, IMediator mediator, LinkGenerator linkGenerator, CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdateInvoiceStatusCommand(organizationId, id, request.Name, request.Handle, request.Description), cancellationToken);

        /*
        if (result.HasError(Errors.Invoices.InvoiceNotFound))
        {
            return TypedResults.NotFound();
        }

        var invoice = result.GetValue();*/

        return TypedResults.Ok();
    }

    private static async Task<Results<Ok, NotFound>> DeleteInvoiceStatus(string organizationId, int id, IMediator mediator = default!, LinkGenerator linkGenerator = default!, CancellationToken cancellationToken = default!)
    {
        await mediator.Send(new DeleteInvoiceStatusCommand(organizationId, id), cancellationToken);

        /*
        if (result.HasError(Errors.Invoices.InvoiceNotFound))
        {
            return TypedResults.NotFound();
        }

        if (result.HasError(Errors.Invoices.InvoiceItemNotFound))
        {
            return TypedResults.NotFound();
        }
        */

        return TypedResults.Ok();
    }
}

public record CreateInvoiceStatusDto(string Name, string Handle, string? Description);

public record UpdateInvoiceStatusDto(string Name, string Handle, string? Description);
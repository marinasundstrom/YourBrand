using MediatR;
using Asp.Versioning;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using YourBrand.Ticketing.Models;
using YourBrand.Ticketing.Application.Features.Tickets.Dtos;
using YourBrand.Ticketing.Application.Features.Tickets.Statuses.Queries;
using YourBrand.Ticketing.Application.Features.Tickets.Statuses.Commands;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Http.HttpResults;

namespace YourBrand.Ticketing.Application.Features.Tickets.Statuses;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/Tickets/Statuses")]
[Authorize]
public sealed class TicketStatusesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResult<TicketStatusDto>))]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesDefaultResponseType]
    public async Task<PagedResult<TicketStatusDto>> GetStatuses(string organizationId, string? searchTerm, int page = 1, int pageSize = 10, string? sortBy = null, SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
        => await mediator.Send(new GetTicketStatusesQuery(organizationId, searchTerm, page, pageSize, sortBy, sortDirection), cancellationToken);

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TicketStatusDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<TicketStatusDto>> GetTicketStatusById(string organizationId, int id, CancellationToken cancellationToken = default)
    {
        var r = await mediator.Send(new GetTicketStatusQuery(organizationId, id), cancellationToken);
        if(r is null) return NotFound();
        return Ok(r);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TicketStatusDto))]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<TicketStatusDto>> CreateTicketStatus(string organizationId, CreateTicketStatusDto request, CancellationToken cancellationToken = default)
    {
        return await mediator.Send(new CreateTicketStatusCommand(organizationId, request.Name, request.Handle, request.Description), cancellationToken);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TicketStatusDto))]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<TicketStatusDto>> UpdateTicketStatus(string organizationId, int id, UpdateTicketStatusDto request, CancellationToken cancellationToken = default)
    {
        return await mediator.Send(new UpdateTicketStatusCommand(organizationId, id, request.Name, request.Handle, request.Description), cancellationToken);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TicketStatusDto))]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesDefaultResponseType]
    public async Task DeleteTicketStatus(string organizationId, int id, CancellationToken cancellationToken = default)
    {
        await mediator.Send(new DeleteTicketStatusCommand(organizationId, id), cancellationToken);
    }
}

public record CreateTicketStatusDto(string Name, string Handle, string? Description);

public record UpdateTicketStatusDto(string Name, string Handle, string? Description);
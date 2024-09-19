using MediatR;
using Asp.Versioning;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using YourBrand.Ticketing.Models;
using YourBrand.Ticketing.Application.Features.Tickets.Commands;
using YourBrand.Ticketing.Application.Features.Tickets.Dtos;
using YourBrand.Ticketing.Application.Features.Tickets.Queries;
using YourBrand.Ticketing.Application.Tickets.Commands;

namespace YourBrand.Ticketing.Application.Features.Tickets;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
[Authorize]
public sealed class TicketsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResult<TicketDto>))]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesDefaultResponseType]
    public async Task<PagedResult<TicketDto>> GetTickets(string organizationId, [FromQuery] int[]? status, string? assigneeId, int page = 1, int pageSize = 10, string? sortBy = null, SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
        => await mediator.Send(new GetTickets(organizationId, status, assigneeId, page, pageSize, sortBy, sortDirection), cancellationToken);

    [HttpGet("{id}/events")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResult<TicketEventDto>))]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesDefaultResponseType]
    public async Task<PagedResult<TicketEventDto>> GetTicketEvents(string organizationId, int id, int page = 1, int pageSize = 10, string? sortBy = null, SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
        => await mediator.Send(new GetTicketEvents(organizationId, id, page, pageSize, sortBy, sortDirection), cancellationToken);

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TicketDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<TicketDto>> GetTicketById(string organizationId, int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetTicketById(organizationId, id), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(TicketDto))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<TicketDto>> CreateTicket(string organizationId, CreateTicketRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateTicket(organizationId, request.Title, request.Text, request.Status, request.AssigneeId, request.EstimatedHours, request.RemainingHours), cancellationToken);
        return result.Handle(
            onSuccess: data => CreatedAtAction(nameof(GetTicketById), new { id = data.Id }, data),
            onError: error => Problem(detail: error.Detail, title: error.Title, type: error.Id));
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> DeleteTicket(string organizationId, int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteTicket(organizationId, id), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPut("{id}/subject")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> UpdateTitle(string organizationId, int id, [FromBody] string title, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateSubject(organizationId, id, title), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPut("{id}/text")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> UpdateText(string organizationId, int id, [FromBody] string? text, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateDescription(organizationId, id, text!), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPut("{id}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> UpdateStatus(string organizationId, int id, [FromBody] int status, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateStatus(organizationId, id, status), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPut("{id}/assignee")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> UpdateAssignee(string organizationId, int id, [FromBody] string? userId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateAssignee(organizationId, id, userId), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPut("{id}/estimatedHours")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> UpdateEstimatedHours(string organizationId, int id, [FromBody] double? hours, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateEstimatedHours(organizationId, id, hours), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPut("{id}/remainingHours")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> UpdateRemainingHours(string organizationId, int id, [FromBody] double? hours, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateRemainingHours(organizationId, id, hours), cancellationToken);
        return this.HandleResult(result);
    }
}

public sealed record CreateTicketRequest(string Title, string? Text, int Status, string? AssigneeId, double? EstimatedHours, double? RemainingHours);
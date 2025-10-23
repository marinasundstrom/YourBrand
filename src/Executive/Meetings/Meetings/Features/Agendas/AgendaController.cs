using System;

using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.Meetings.Domain.ValueObjects;
using YourBrand.Meetings.Features.Agendas.Command;
using YourBrand.Meetings.Features.Agendas.Queries;
using YourBrand.Meetings.Features.Command;
using YourBrand.Meetings.Models;

namespace YourBrand.Meetings.Features.Agendas;

public sealed record CreateAgendaDto(IEnumerable<CreateAgendaItemDto> Items);

public sealed record EditAgendaDetailsDto();

public sealed record AddAgendaItemDto(int Type, string Title, string Description, int? MotionId, int? Order, TimeSpan? EstimatedStartTime, TimeSpan? EstimatedEndTime, TimeSpan? EstimatedDuration);

public sealed record EditAgendaItemDto(int Type, string Title, string Description, int? MotionId, TimeSpan? EstimatedStartTime, TimeSpan? EstimatedEndTime, TimeSpan? EstimatedDuration);

public sealed record ReorderAgendaItemDto(int Order);

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
public sealed class AgendasController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResult<AgendaDto>))]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesDefaultResponseType]
    public async Task<PagedResult<AgendaDto>> GetAgendas(string organizationId, int? meetingId = null, int page = 1, int pageSize = 10, string? searchTerm = null, string? sortBy = null, SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
        => await mediator.Send(new GetAgenda(organizationId, meetingId, page, pageSize, searchTerm, sortBy, sortDirection), cancellationToken);

    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AgendaDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<AgendaDto>> CreateAgenda(string organizationId, int meetingId, CreateAgendaDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateAgenda(organizationId, meetingId, request.Items), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AgendaDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<AgendaDto>> GetAgendaById(string organizationId, int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAgendaById(organizationId, id), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPut("{id}/details")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AgendaDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<AgendaDto>> EditAgendaDetails(string organizationId, int id, EditAgendaDetailsDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new EditAgendaDetails(organizationId, id), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPost("{id}/items")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AgendaItemDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<AgendaItemDto>> AddAgendaItem(string organizationId, int id, AddAgendaItemDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new AddAgendaItem(organizationId, id, request.Type, request.Title, request.Description, request.MotionId, request.Order, request.EstimatedStartTime, request.EstimatedEndTime, request.EstimatedDuration), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPut("{id}/items/{itemId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AgendaItemDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<AgendaItemDto>> EditAgendaItem(string organizationId, int id, string itemId, EditAgendaItemDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new EditAgendaItem(organizationId, id, itemId, request.Type, request.Title, request.Description, request.MotionId, request.EstimatedStartTime, request.EstimatedEndTime, request.EstimatedDuration), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPut("{id}/items/{itemId}/order")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AgendaItemDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<AgendaItemDto>> ReorderAgendaItem(string organizationId, int id, string itemId, ReorderAgendaItemDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ReorderAgendaItem(organizationId, id, itemId, request.Order), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpDelete("{id}/items/{itemId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> RemoveAgendaItem(string organizationId, int id, string itemId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RemoveAgendaItem(organizationId, id, itemId), cancellationToken);
        return this.HandleResult(result);
    }
}
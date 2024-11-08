using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.Meetings.Domain.ValueObjects;
using YourBrand.Meetings.Features.Command;
using YourBrand.Meetings.Features.Minutes.Command;
using YourBrand.Meetings.Features.Minutes.Queries;
using YourBrand.Meetings.Models;

namespace YourBrand.Meetings.Features.Minutes;

public sealed record CreateMinutesDto(int MeetingId, IEnumerable<CreateMinutesItemDto> Items);

public sealed record EditMinuteDetailsDto();

public sealed record AddMinutesItemDto(int? AgendaId, string? AgendaItemId, int Type, string Title, string Description, int? MotionId, int? Order);

public sealed record EditMinutesItemDto(int Type, string Title, string Description, int? MotionId);

public sealed record MoveMinutesItemDto(int Order);

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
public sealed class MinutesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResult<MinutesDto>))]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesDefaultResponseType]
    public async Task<PagedResult<MinutesDto>> GetMinutes(string organizationId, int? meetingId = null, int page = 1, int pageSize = 10, string? searchTerm = null, string? sortBy = null, SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
        => await mediator.Send(new GetMinutes(organizationId, meetingId, page, pageSize, searchTerm, sortBy, sortDirection), cancellationToken);

    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MinutesDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<MinutesDto>> CreateMinutes(string organizationId, CreateMinutesDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateMinutes(organizationId, request.MeetingId, request.Items), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MinutesDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<MinutesDto>> GetMinutesById(string organizationId, int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetMinutesById(organizationId, id), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPut("{id}/details")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MinutesDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<MinutesDto>> EditMinutesDetails(string organizationId, int id, EditMinuteDetailsDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new EditMinutesDetails(organizationId, id), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPost("{id}/items")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MinutesItemDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<MinutesItemDto>> AddMinutesItem(string organizationId, int id, AddMinutesItemDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new AddMinutesItem(organizationId, id, request.AgendaId, request.AgendaItemId, request.Type, request.Title, request.Description, request.MotionId, request.Order), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPut("{id}/items/{itemId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MinutesItemDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<MinutesItemDto>> EditMinutesItem(string organizationId, int id, string itemId, EditMinutesItemDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new EditMinutesItem(organizationId, id, itemId, request.Type, request.Title, request.Description, request.MotionId), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPut("{id}/items/{itemId}/order")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MinutesItemDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<MinutesItemDto>> MoveMinutesItem(string organizationId, int id, string itemId, MoveMinutesItemDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new MoveMinutesItem(organizationId, id, itemId, request.Order), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpDelete("{id}/items/{itemId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> RemoveMinutesItem(string organizationId, int id, string itemId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RemoveMinutesItem(organizationId, id, itemId), cancellationToken);
        return this.HandleResult(result);
    }
}
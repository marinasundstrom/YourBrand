using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.Meetings.Domain.ValueObjects;
using YourBrand.Meetings.Features.Motions.Command;
using YourBrand.Meetings.Features.Motions.Queries;
using YourBrand.Meetings.Features.Command;
using YourBrand.Meetings.Models;

namespace YourBrand.Meetings.Features.Motions;

public sealed record CreateMotionDto(string Title, string Text, IEnumerable<CreateOperativeClauseDto> Items);

public sealed record EditMotionDto(string Title, string Text);

public sealed record ChangeMotionStatusDto(MotionStatus Status);

public sealed record AddOperativeClauseDto(OperativeAction Action, string Text);

public sealed record EditOperativeClauseDto(OperativeAction Action, string Text);

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
public sealed class MotionsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResult<MotionDto>))]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesDefaultResponseType]
    public async Task<PagedResult<MotionDto>> GetMotions(string organizationId, int? meetingId = null, int page = 1, int pageSize = 10, string? searchTerm = null, string? sortBy = null, SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
        => await mediator.Send(new GetMotion(organizationId, meetingId, page, pageSize, searchTerm, sortBy, sortDirection), cancellationToken);

    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MotionDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<MotionDto>> CreateMotion(string organizationId, CreateMotionDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateMotion(organizationId, request.Title, request.Text, request.Items), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MotionDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<MotionDto>> GetMotionById(string organizationId, int id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetMotionById(organizationId, id), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPut("{id}/details")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MotionDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<MotionDto>> EditMotionDetails(string organizationId, int id, EditMotionDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new EditMotionDetails(organizationId, id, request.Title, request.Text), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPut("{id}/status")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MotionDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<MotionDto>> ChangeMotionStatus(string organizationId, int id, ChangeMotionStatusDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ChangeMotionStatus(organizationId, id, request.Status), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPost("{id}/OperativeClauses")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MotionOperativeClauseDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<MotionOperativeClauseDto>> AddOperativeClause(string organizationId, int id, AddOperativeClauseDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new AddOperativeClause(organizationId, id, request.Action, request.Text), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPut("{id}/OperativeClauses/{clauseId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MotionOperativeClauseDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<MotionOperativeClauseDto>> EditOperativeClause(string organizationId, int id, string clauseId, EditOperativeClauseDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new EditOperativeClause(organizationId, id, clauseId, request.Action, request.Text), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpDelete("{id}/OperativeClauses/{clauseId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> RemoveOperativeClause(string organizationId, int id, string clauseId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RemoveOperativeClause(organizationId, id, clauseId), cancellationToken);
        return this.HandleResult(result);
    }
}
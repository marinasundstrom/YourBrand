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

public sealed record CreateMotionDto(string Title, string Text, IEnumerable<CreateMotionItemDto> Items);

public sealed record EditMotionDto(string Title, string Text);

public sealed record ChangeMotionStatusDto(MotionStatus Status);

public sealed record AddMotionItemDto(string Text);

public sealed record EditMotionItemDto(string Text);

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

    [HttpPost("{id}/items")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MotionItemDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<MotionItemDto>> AddMotionItem(string organizationId, int id, AddMotionItemDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new AddMotionItem(organizationId, id, request.Text), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPut("{id}/items/{itemId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MotionItemDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<MotionItemDto>> EditMotionItem(string organizationId, int id, string itemId, EditMotionItemDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new EditMotionItem(organizationId, id, itemId, request.Text), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpDelete("{id}/items/{itemId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult> RemoveMotionItem(string organizationId, int id, string itemId, CancellationToken cancellationToken)
    {
        await mediator.Send(new RemoveMotionItem(organizationId, id, itemId), cancellationToken);
        return Ok();
    }
}
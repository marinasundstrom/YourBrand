using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.ApiKeys;
using YourBrand.Domain;
using YourBrand.TimeReport.Application.Common.Models;
using YourBrand.TimeReport.Application.TimeSheets;
using YourBrand.TimeReport.Application.TimeSheets.Commands;
using YourBrand.TimeReport.Application.TimeSheets.Queries;
using YourBrand.TimeReport.Dtos;

namespace YourBrand.TimeReport.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class TimeSheetsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ItemsResult<TimeSheetDto>>> GetTimeSheets(string organizationId, int page = 0, int pageSize = 10, string? projectId = null, string? userId = null, string? searchString = null, string? sortBy = null, TimeReport.Application.Common.Models.SortDirection? sortDirection = null)
    {
        return Ok(await mediator.Send(new GetTimeSheetsQuery(organizationId, page, pageSize, projectId, userId, searchString, sortBy, sortDirection)));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<TimeSheetDto>> GetTimeSheet(string organizationId, [FromRoute] string id, CancellationToken cancellationToken)
    {
        var timeSheet = await mediator.Send(new GetTimeSheetQuery(organizationId, id), cancellationToken);

        if (timeSheet is null)
        {
            return NotFound();
        }

        return Ok(timeSheet);
    }

    [HttpGet("{year}/{week}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<TimeSheetDto>> GetTimeSheetForWeek(string organizationId, [FromRoute] int year, [FromRoute] int week, [FromQuery] string? userId, CancellationToken cancellationToken)
    {
        var r = await mediator.Send(new GetTimeSheetForWeekQuery(organizationId, year, week, userId), cancellationToken);
        return this.HandleResult(r);
    }

    [HttpPost("{timeSheetId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EntryDto>> CreateEntry(string organizationId, [FromRoute] string timeSheetId, CreateEntryDto dto, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateEntryCommand(organizationId, timeSheetId,  dto.ProjectId, dto.ActivityId, DateOnly.FromDateTime(dto.Date), dto.Hours, dto.Description), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPut("{timeSheetId}/{entryId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EntryDto>> UpdateEntry(string organizationId, [FromRoute] string timeSheetId, [FromRoute] string entryId, UpdateEntryDto dto, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateEntryCommand(organizationId, timeSheetId, entryId, dto.Hours, dto.Description), cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPut("{timeSheetId}/{entryId}/Details")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EntryDto>> UpdateEntryDetails(string organizationId, [FromRoute] string timeSheetId, [FromRoute] string entryId, UpdateEntryDetailsDto dto, CancellationToken cancellationToken)
    {
        var r = await mediator.Send(new UpdateEntryDetailsCommand(organizationId, timeSheetId, entryId, dto.Description), cancellationToken);
        return this.HandleResult(r);
    }

    [HttpDelete("{timeSheetId}/{activityId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> DeleteActvityEntries(string organizationId, [FromRoute] string timeSheetId, [FromRoute] string activityId, CancellationToken cancellationToken)
    {
        var r = await mediator.Send(new DeleteActivityCommand(organizationId, timeSheetId, activityId), cancellationToken);
        return this.HandleResult(r);
    }

    [HttpPost("{timeSheetId}/CloseWeek")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> CloseWeek(string organizationId, [FromRoute] string timeSheetId, CancellationToken cancellationToken)
    {
        var r = await mediator.Send(new CloseWeekCommand(organizationId, timeSheetId), cancellationToken);
        return this.HandleResult(r);
    }

    [HttpPost("{timeSheetId}/OpenWeek")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> OpenWeek(string organizationId, [FromRoute] string timeSheetId, CancellationToken cancellationToken)
    {
        var r = await mediator.Send(new ReopenWeekCommand(organizationId, timeSheetId), cancellationToken);
        return this.HandleResult(r);
    }

    [HttpPost("{timeSheetId}/Approve")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> ApproveWeek(string organizationId, [FromRoute] string timeSheetId, CancellationToken cancellationToken)
    {
        var r = await mediator.Send(new ApproveWeekCommand(organizationId, timeSheetId), cancellationToken);
        return this.HandleResult(r);
    }

    [HttpPut("{timeSheetId}/Status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> UpdateTimeSheetStatus(string organizationId, [FromRoute] string timeSheetId, int statusCode, CancellationToken cancellationToken)
    {
        var r = await mediator.Send(new UpdateTimeSheetStatusCommand(organizationId, timeSheetId), cancellationToken);
        return this.HandleResult(r);
    }

    [HttpPost("{timeSheetId}/LockMonth")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> LockMonth(string organizationId, [FromRoute] string timeSheetId, CancellationToken cancellationToken)
    {
        var r = await mediator.Send(new LockMonthCommand(organizationId, timeSheetId), cancellationToken);
        return this.HandleResult(r);
    }
}
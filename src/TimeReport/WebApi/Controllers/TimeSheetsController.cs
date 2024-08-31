using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.ApiKeys;
using YourBrand.Domain;
using YourBrand.TimeReport.Application.Common.Models;
using YourBrand.TimeReport.Application.TimeSheets;
using YourBrand.TimeReport.Application.TimeSheets.Commands;
using YourBrand.TimeReport.Application.TimeSheets.Queries;
using YourBrand.TimeReport.Domain.Exceptions;
using YourBrand.TimeReport.Dtos;

using static System.Result<YourBrand.TimeReport.Application.TimeSheets.EntryDto, YourBrand.TimeReport.Domain.Exceptions.DomainException>;

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
        return Ok(await mediator.Send(new GetTimeSheetForWeekQuery(organizationId, year, week, userId), cancellationToken));
    }

    [HttpPost("{timeSheetId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EntryDto>> CreateEntry(string organizationId, [FromRoute] string timeSheetId, CreateEntryDto dto, CancellationToken cancellationToken)
    {
        var date = DateOnly.FromDateTime(dto.Date);

        var result = await mediator.Send(new CreateEntryCommand(organizationId, timeSheetId, dto.ProjectId, dto.ActivityId, DateOnly.FromDateTime(dto.Date), dto.Hours, dto.Description), cancellationToken);

        return result switch
        {
            Ok(EntryDto value) => Ok(value),
            Error(DomainException exc) => Problem(title: exc.Title, detail: exc.Details, statusCode: StatusCodes.Status400BadRequest),
            _ => BadRequest()

            /*
            Result<EntryDto, DomainException>.Error(DomainException exception) when exception is TimeSheetClosedException exc => Problem(title: exc.Title, detail: exc.Details, statusCode: StatusCodes.Status400BadRequest),
            Result<EntryDto, DomainException>.Error(DomainException exception) when exception is MonthLockedException exc => Problem(title: exc.Title, detail: exc.Details, statusCode: StatusCodes.Status400BadRequest),
            Result<EntryDto, DomainException>.Error(DomainException exception) when exception is EntryAlreadyExistsException exc => Problem(title: exc.Title, detail: exc.Details, statusCode: StatusCodes.Status400BadRequest),
            Result<EntryDto, DomainException>.Error(DomainException exception) when exception is ProjectMembershipNotFoundException exc => Problem(title: exc.Title, detail: exc.Details, statusCode: StatusCodes.Status400BadRequest),
            Result<EntryDto, DomainException>.Error(DomainException exception) when exception is ActivityNotFoundException exc => Problem(title: exc.Title, detail: exc.Details, statusCode: StatusCodes.Status400BadRequest),
            Result<EntryDto, DomainException>.Error(DomainException exception) when exception is DayHoursExceedPermittedDailyWorkingHoursException exc => Problem(title: exc.Title, detail: exc.Details, statusCode: StatusCodes.Status400BadRequest),
            Result<EntryDto, DomainException>.Error(DomainException exception) when exception is WeekHoursExceedPermittedWeeklyWorkingHoursException exc => Problem(title: exc.Title, detail: exc.Details, statusCode: StatusCodes.Status400BadRequest),
            */
        };
    }

    [HttpPut("{timeSheetId}/{entryId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EntryDto>> UpdateEntry(string organizationId, [FromRoute] string timeSheetId, [FromRoute] string entryId, UpdateEntryDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var result = await mediator.Send(new UpdateEntryCommand(organizationId, timeSheetId, entryId, dto.Hours, dto.Description), cancellationToken);

            return result switch
            {
                Ok(EntryDto value) => Ok(value),
                Error(DomainException exc) => Problem(title: exc.Title, detail: exc.Details, statusCode: StatusCodes.Status400BadRequest),
                _ => BadRequest()
            };
        }
        catch (TimeSheetNotFoundException exc)
        {
            return Problem(title: exc.Title, detail: exc.Details, statusCode: StatusCodes.Status400BadRequest);
        }
        catch (TimeSheetClosedException exc)
        {
            return Problem(title: exc.Title, detail: exc.Details, statusCode: StatusCodes.Status400BadRequest);
        }
        catch (MonthLockedException exc)
        {
            return Problem(title: exc.Title, detail: exc.Details, statusCode: StatusCodes.Status400BadRequest);
        }
        catch (EntryAlreadyExistsException exc)
        {
            return Problem(title: exc.Title, detail: exc.Details, statusCode: StatusCodes.Status400BadRequest);
        }
        catch (DayHoursExceedPermittedDailyWorkingHoursException exc)
        {
            return Problem(title: exc.Title, detail: exc.Details, statusCode: StatusCodes.Status400BadRequest);
        }
        catch (WeekHoursExceedPermittedWeeklyWorkingHoursException exc)
        {
            return Problem(title: exc.Title, detail: exc.Details, statusCode: StatusCodes.Status400BadRequest);
        }
    }

    [HttpPut("{timeSheetId}/{entryId}/Details")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EntryDto>> UpdateEntryDetails(string organizationId, [FromRoute] string timeSheetId, [FromRoute] string entryId, UpdateEntryDetailsDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var newDto = await mediator.Send(new UpdateEntryDetailsCommand(organizationId, timeSheetId, entryId, dto.Description), cancellationToken);
            return Ok(newDto);
        }
        catch (TimeSheetNotFoundException exc)
        {
            return Problem(title: exc.Title, detail: exc.Details, statusCode: StatusCodes.Status400BadRequest);
        }
        catch (TimeSheetClosedException exc)
        {
            return Problem(title: exc.Title, detail: exc.Details, statusCode: StatusCodes.Status400BadRequest);
        }
        catch (MonthLockedException exc)
        {
            return Problem(title: exc.Title, detail: exc.Details, statusCode: StatusCodes.Status400BadRequest);
        }
        catch (EntryAlreadyExistsException exc)
        {
            return Problem(title: exc.Title, detail: exc.Details, statusCode: StatusCodes.Status400BadRequest);
        }
        catch (DayHoursExceedPermittedDailyWorkingHoursException exc)
        {
            return Problem(title: exc.Title, detail: exc.Details, statusCode: StatusCodes.Status400BadRequest);
        }
        catch (WeekHoursExceedPermittedWeeklyWorkingHoursException exc)
        {
            return Problem(title: exc.Title, detail: exc.Details, statusCode: StatusCodes.Status400BadRequest);
        }
    }

    [HttpDelete("{timeSheetId}/{activityId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> DeleteActvityEntries(string organizationId, [FromRoute] string timeSheetId, [FromRoute] string activityId, CancellationToken cancellationToken)
    {
        try
        {
            await mediator.Send(new DeleteActivityCommand(organizationId, timeSheetId, activityId), cancellationToken);
            return Ok();
        }
        catch (TimeSheetNotFoundException exc)
        {
            return Problem(title: exc.Title, detail: exc.Details, statusCode: StatusCodes.Status400BadRequest);
        }
        catch (TimeSheetClosedException exc)
        {
            return Problem(title: exc.Title, detail: exc.Details, statusCode: StatusCodes.Status400BadRequest);
        }
        catch (MonthLockedException exc)
        {
            return Problem(title: exc.Title, detail: exc.Details, statusCode: StatusCodes.Status400BadRequest);
        }
    }

    [HttpPost("{timeSheetId}/CloseWeek")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> CloseWeek(string organizationId, [FromRoute] string timeSheetId, CancellationToken cancellationToken)
    {
        try
        {
            await mediator.Send(new CloseWeekCommand(organizationId, timeSheetId), cancellationToken);
            return Ok();
        }
        catch (TimeSheetNotFoundException exc)
        {
            return Problem(title: exc.Title, detail: exc.Details, statusCode: StatusCodes.Status400BadRequest);
        }
    }

    [HttpPost("{timeSheetId}/OpenWeek")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> OpenWeek(string organizationId, [FromRoute] string timeSheetId, CancellationToken cancellationToken)
    {
        try
        {
            await mediator.Send(new ReopenWeekCommand(organizationId, timeSheetId), cancellationToken);
            return Ok();
        }
        catch (TimeSheetNotFoundException exc)
        {
            return Problem(title: exc.Title, detail: exc.Details, statusCode: StatusCodes.Status400BadRequest);
        }
    }

    [HttpPost("{timeSheetId}/Approve")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> ApproveWeek(string organizationId, [FromRoute] string timeSheetId, CancellationToken cancellationToken)
    {
        try
        {
            await mediator.Send(new ApproveWeekCommand(organizationId, timeSheetId), cancellationToken);
            return Ok();
        }
        catch (TimeSheetNotFoundException exc)
        {
            return Problem(title: exc.Title, detail: exc.Details, statusCode: StatusCodes.Status400BadRequest);
        }
    }

    [HttpPut("{timeSheetId}/Status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> UpdateTimeSheetStatus(string organizationId, [FromRoute] string timeSheetId, int statusCode, CancellationToken cancellationToken)
    {
        try
        {
            await mediator.Send(new UpdateTimeSheetStatusCommand(organizationId, timeSheetId), cancellationToken);
            return Ok();
        }
        catch (TimeSheetNotFoundException exc)
        {
            return Problem(title: exc.Title, detail: exc.Details, statusCode: StatusCodes.Status400BadRequest);
        }
    }

    [HttpPost("{timeSheetId}/LockMonth")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> LockMonth(string organizationId, [FromRoute] string timeSheetId, CancellationToken cancellationToken)
    {
        try
        {
            await mediator.Send(new LockMonthCommand(organizationId, timeSheetId), cancellationToken);
            return Ok();
        }
        catch (TimeSheetNotFoundException exc)
        {
            return Problem(title: exc.Title, detail: exc.Details, statusCode: StatusCodes.Status400BadRequest);
        }
    }
}
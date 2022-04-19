
using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.ApiKeys;
using YourBrand.TimeReport.Application.Common.Models;
using YourBrand.TimeReport.Application.TimeSheets;
using YourBrand.TimeReport.Application.TimeSheets.Commands;
using YourBrand.TimeReport.Application.TimeSheets.Queries;
using YourBrand.TimeReport.Domain.Exceptions;
using YourBrand.TimeReport.Dtos;

namespace YourBrand.TimeReport.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(AuthenticationSchemes = AuthSchemes.Default)]
public class TimeSheetsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TimeSheetsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ItemsResult<TimeSheetDto>>> GetTimeSheets(int page = 0, int pageSize = 10, string? projectId = null, string? userId = null, string? searchString = null, string? sortBy = null, TimeReport.Application.Common.Models.SortDirection? sortDirection = null)
    {
        return Ok(await _mediator.Send(new GetTimeSheetsQuery(page, pageSize, projectId, userId, searchString, sortBy, sortDirection)));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<TimeSheetDto>> GetTimeSheet([FromRoute] string id, CancellationToken cancellationToken)
    {
        var timeSheet = await _mediator.Send(new GetTimeSheetQuery(id), cancellationToken);

        if (timeSheet is null)
        {
            return NotFound();
        }

        return Ok(timeSheet);
    }

    [HttpGet("{year}/{week}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<TimeSheetDto>> GetTimeSheetForWeek([FromRoute] int year, [FromRoute] int week, [FromQuery] string? userId, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new GetTimeSheetForWeekQuery(year, week, userId), cancellationToken));
    }

    [HttpPost("{timeSheetId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EntryDto>> CreateEntry([FromRoute] string timeSheetId, CreateEntryDto dto, CancellationToken cancellationToken)
    {
        var date = DateOnly.FromDateTime(dto.Date);

        var result = await _mediator.Send(new CreateEntryCommand(timeSheetId, dto.ProjectId, dto.ActivityId, DateOnly.FromDateTime(dto.Date), dto.Hours, dto.Description), cancellationToken);

        return result switch
        {
            ResultWithValue<EntryDto, DomainException>.Ok(EntryDto value) => Ok(value),
            ResultWithValue<EntryDto, DomainException>.Error(DomainException exc) => Problem(title: exc.Title, detail: exc.Details, statusCode: StatusCodes.Status400BadRequest),
            _ => BadRequest()

            /*
            ResultWithValue<EntryDto, DomainException>.Error(DomainException exception) when exception is TimeSheetClosedException exc => Problem(title: exc.Title, detail: exc.Details, statusCode: StatusCodes.Status400BadRequest),
            ResultWithValue<EntryDto, DomainException>.Error(DomainException exception) when exception is MonthLockedException exc => Problem(title: exc.Title, detail: exc.Details, statusCode: StatusCodes.Status400BadRequest),
            ResultWithValue<EntryDto, DomainException>.Error(DomainException exception) when exception is EntryAlreadyExistsException exc => Problem(title: exc.Title, detail: exc.Details, statusCode: StatusCodes.Status400BadRequest),
            ResultWithValue<EntryDto, DomainException>.Error(DomainException exception) when exception is ProjectMembershipNotFoundException exc => Problem(title: exc.Title, detail: exc.Details, statusCode: StatusCodes.Status400BadRequest),
            ResultWithValue<EntryDto, DomainException>.Error(DomainException exception) when exception is ActivityNotFoundException exc => Problem(title: exc.Title, detail: exc.Details, statusCode: StatusCodes.Status400BadRequest),
            ResultWithValue<EntryDto, DomainException>.Error(DomainException exception) when exception is DayHoursExceedPermittedDailyWorkingHoursException exc => Problem(title: exc.Title, detail: exc.Details, statusCode: StatusCodes.Status400BadRequest),
            ResultWithValue<EntryDto, DomainException>.Error(DomainException exception) when exception is WeekHoursExceedPermittedWeeklyWorkingHoursException exc => Problem(title: exc.Title, detail: exc.Details, statusCode: StatusCodes.Status400BadRequest),
            */
        };
    }

    [HttpPut("{timeSheetId}/{entryId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EntryDto>> UpdateEntry([FromRoute] string timeSheetId, [FromRoute] string entryId, UpdateEntryDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(new UpdateEntryCommand(timeSheetId, entryId, dto.Hours, dto.Description), cancellationToken);
            
            return result switch
            {
                ResultWithValue<EntryDto, DomainException>.Ok(EntryDto value) => Ok(value),
                ResultWithValue<EntryDto, DomainException>.Error(DomainException exc) => Problem(title: exc.Title, detail: exc.Details, statusCode: StatusCodes.Status400BadRequest),
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
    public async Task<ActionResult<EntryDto>> UpdateEntryDetails([FromRoute] string timeSheetId, [FromRoute] string entryId, UpdateEntryDetailsDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var newDto = await _mediator.Send(new UpdateEntryDetailsCommand(timeSheetId, entryId, dto.Description), cancellationToken);
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
    public async Task<ActionResult> DeleteActvityEntries([FromRoute] string timeSheetId, [FromRoute] string activityId, CancellationToken cancellationToken)
    {
        try
        {
            var newDto = await _mediator.Send(new DeleteActivityCommand(timeSheetId, activityId), cancellationToken);
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
    public async Task<ActionResult> CloseWeek([FromRoute] string timeSheetId, CancellationToken cancellationToken)
    {
        try
        {
            await _mediator.Send(new CloseWeekCommand(timeSheetId), cancellationToken);
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
    public async Task<ActionResult> OpenWeek([FromRoute] string timeSheetId, CancellationToken cancellationToken)
    {
        try
        {
            await _mediator.Send(new OpenWeekCommand(timeSheetId), cancellationToken);
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
    public async Task<ActionResult> ApproveWeek([FromRoute] string timeSheetId, CancellationToken cancellationToken)
    {
        try
        {
            await _mediator.Send(new ApproveWeekCommand(timeSheetId), cancellationToken);
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
    public async Task<ActionResult> UpdateTimeSheetStatus([FromRoute] string timeSheetId, int statusCode, CancellationToken cancellationToken)
    {
        try
        {
            await _mediator.Send(new UpdateTimeSheetStatusCommand(timeSheetId), cancellationToken);
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
    public async Task<ActionResult> LockMonth([FromRoute] string timeSheetId, CancellationToken cancellationToken)
    {
        try
        {
            await _mediator.Send(new LockMonthCommand(timeSheetId), cancellationToken);
            return Ok();
        }
        catch (TimeSheetNotFoundException exc)
        {
            return Problem(title: exc.Title, detail: exc.Details, statusCode: StatusCodes.Status400BadRequest);
        }
    }
}
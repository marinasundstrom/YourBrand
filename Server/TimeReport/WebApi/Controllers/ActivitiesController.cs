
using MediatR;

using Microsoft.AspNetCore.Mvc;

using TimeReport.Application.Activities;
using TimeReport.Application.Activities.Commands;
using TimeReport.Application.Activities.Queries;
using TimeReport.Application.Common.Interfaces;
using TimeReport.Application.Common.Models;

namespace TimeReport.Controllers;

[ApiController]
[Route("[controller]")]
public class ActivitiesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ActivitiesController(IMediator mediator, ITimeReportContext context)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ItemsResult<ActivityDto>>> GetActivities(int page = 0, int pageSize = 10, string? projectId = null, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new GetActivitiesQuery(page, pageSize, projectId, searchString, sortBy, sortDirection)));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ActivityDto>> GetActivity(string id, CancellationToken cancellationToken)
    {
        var activity = await _mediator.Send(new GetActivityQuery(id), cancellationToken);

        if (activity is null)
        {
            return NotFound();
        }

        return Ok(activity);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ActivityDto>> CreateActivity(string projectId, CreateActivityDto createActivityDto, CancellationToken cancellationToken)
    {
        try
        {
            var activity = await _mediator.Send(new CreateActivityCommand(projectId, createActivityDto.Name, createActivityDto.Description, createActivityDto.HourlyRate), cancellationToken);

            return Ok(activity);
        }
        catch (Exception)
        {
            return NotFound();
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ActivityDto>> UpdateActivity(string id, UpdateActivityDto updateActivityDto, CancellationToken cancellationToken)
    {
        try
        {
            var activity = await _mediator.Send(new UpdateActivityCommand(id, updateActivityDto.Name, updateActivityDto.Description, updateActivityDto.HourlyRate), cancellationToken);

            return Ok(activity);
        }
        catch (Exception)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> DeleteActivity(string id, CancellationToken cancellationToken)
    {
        try
        {
            var activity = await _mediator.Send(new DeleteActivityCommand(id), cancellationToken);

            return Ok();
        }
        catch (Exception)
        {
            return NotFound();
        }
    }

    [HttpGet("{id}/Statistics/Summary")]
    public async Task<ActionResult<StatisticsSummary>> GetStatisticsSummary(string id, CancellationToken cancellationToken)
    {
        try
        {
            var statistics = await _mediator.Send(new GetActivityStatisticsSummaryQuery(id), cancellationToken);

            return Ok(_mediator.Send(new GetActivityStatisticsSummaryQuery(id)));
        }
        catch (Exception)
        {
            return NotFound();
        }
    }
}

public record class CreateActivityDto(string Name, string? Description, decimal? HourlyRate);

public record class UpdateActivityDto(string Name, string? Description, decimal? HourlyRate);
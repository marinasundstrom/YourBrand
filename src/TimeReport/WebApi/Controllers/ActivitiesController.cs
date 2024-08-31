using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.ApiKeys;
using YourBrand.TimeReport.Application.Activities;
using YourBrand.TimeReport.Application.Activities.Commands;
using YourBrand.TimeReport.Application.Activities.Queries;
using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Common.Models;

namespace YourBrand.TimeReport.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ActivitiesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ItemsResult<ActivityDto>>> GetActivities(string organizationId, int page = 0, int pageSize = 10, string? projectId = null, string? userId = null, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new GetActivitiesQuery(organizationId, page, pageSize, projectId, userId, searchString, sortBy, sortDirection)));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ActivityDto>> GetActivity(string organizationId, string id, CancellationToken cancellationToken)
    {
        var activity = await mediator.Send(new GetActivityQuery(organizationId, id), cancellationToken);

        if (activity is null)
        {
            return NotFound();
        }

        return Ok(activity);
    }

    [HttpPost]
    [Authorize(Roles = Roles.AdministratorManager)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ActivityDto>> CreateActivity(string organizationId, string projectId, CreateActivityDto createActivityDto, CancellationToken cancellationToken)
    {
        try
        {
            var activity = await mediator.Send(new CreateActivityCommand(organizationId, projectId, createActivityDto.Name, createActivityDto.ActivityTypeId, createActivityDto.Description, createActivityDto.HourlyRate), cancellationToken);

            return Ok(activity);
        }
        catch (Exception)
        {
            return NotFound();
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ActivityDto>> UpdateActivity(string organizationId, string id, UpdateActivityDto updateActivityDto, CancellationToken cancellationToken)
    {
        try
        {
            var activity = await mediator.Send(new UpdateActivityCommand(organizationId, id, updateActivityDto.Name, updateActivityDto.ActivityTypeId, updateActivityDto.Description, updateActivityDto.HourlyRate), cancellationToken);

            return Ok(activity);
        }
        catch (Exception)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.AdministratorManager)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> DeleteActivity(string organizationId, string id, CancellationToken cancellationToken)
    {
        try
        {
            await mediator.Send(new DeleteActivityCommand(organizationId, id), cancellationToken);

            return Ok();
        }
        catch (Exception)
        {
            return NotFound();
        }
    }

    [HttpGet("{id}/Statistics/Summary")]
    public async Task<ActionResult<StatisticsSummary>> GetStatisticsSummary(string organizationId, string id, CancellationToken cancellationToken)
    {
        try
        {
            var statistics = await mediator.Send(new GetActivityStatisticsSummaryQuery(organizationId, id), cancellationToken);

            return Ok(mediator.Send(new GetActivityStatisticsSummaryQuery(organizationId, id)));
        }
        catch (Exception)
        {
            return NotFound();
        }
    }
}

public record class CreateActivityDto(string Name, string ActivityTypeId, string? Description, decimal? HourlyRate);

public record class UpdateActivityDto(string Name, string ActivityTypeId, string? Description, decimal? HourlyRate);
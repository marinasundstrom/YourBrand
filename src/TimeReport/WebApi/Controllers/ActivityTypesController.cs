using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.ApiKeys;
using YourBrand.TimeReport.Application.Activities.ActivityTypes;
using YourBrand.TimeReport.Application.Activities.ActivityTypes.Commands;
using YourBrand.TimeReport.Application.Activities.ActivityTypes.Queries;
using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Common.Models;

namespace YourBrand.TimeReport.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ActivityTypesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ItemsResult<ActivityTypeDto>>> GetActivityTypes(string organizationId, int page = 0, int pageSize = 10, string? projectId = null, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new GetActivityTypesQuery(organizationId, page, pageSize, projectId, searchString, sortBy, sortDirection)));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ActivityTypeDto>> GetActivityType(string organizationId, string id, CancellationToken cancellationToken)
    {
        var activity = await mediator.Send(new GetActivityTypeQuery(organizationId, id), cancellationToken);

        if (activity is null)
        {
            return NotFound();
        }

        return Ok(activity);
    }

    [HttpPost]
    [Authorize(Roles = Roles.AdministratorManager)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ActivityTypeDto>> CreateActivityType(string organizationId, CreateActivityTypeDto createActivityTypeDto, CancellationToken cancellationToken)
    {
        try
        {
            var activity = await mediator.Send(new CreateActivityTypeCommand(organizationId, createActivityTypeDto.Name, createActivityTypeDto.Description, createActivityTypeDto.ProjectId, createActivityTypeDto.ExcludeHours), cancellationToken);

            return Ok(activity);
        }
        catch (Exception)
        {
            return NotFound();
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ActivityTypeDto>> UpdateActivityType(string organizationId, string id, UpdateActivityTypeDto updateActivityTypeDto, CancellationToken cancellationToken)
    {
        try
        {
            var activity = await mediator.Send(new UpdateActivityTypeCommand(organizationId, id, updateActivityTypeDto.Name, updateActivityTypeDto.Description, updateActivityTypeDto.ProjectId, updateActivityTypeDto.ExcludeHours), cancellationToken);

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
    public async Task<ActionResult> DeleteActivityType(string organizationId, string id, CancellationToken cancellationToken)
    {
        try
        {
            await mediator.Send(new DeleteActivityTypeCommand(organizationId, id), cancellationToken);

            return Ok();
        }
        catch (Exception)
        {
            return NotFound();
        }
    }
}

public record class CreateActivityTypeDto(string Name, string? Description, string OrganizationId, string? ProjectId, bool ExcludeHours);

public record class UpdateActivityTypeDto(string Name, string? Description, string OrganizationId, string? ProjectId, bool ExcludeHours);
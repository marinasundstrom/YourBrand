using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.ApiKeys;
using YourBrand.TimeReport.Application.Tasks.TaskTypes;
using YourBrand.TimeReport.Application.Tasks.TaskTypes.Commands;
using YourBrand.TimeReport.Application.Tasks.TaskTypes.Queries;
using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Common.Models;

namespace YourBrand.TimeReport.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class TaskTypesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ItemsResult<TaskTypeDto>>> GetTaskTypes(string organizationId, int page = 0, int pageSize = 10, string? projectId = null, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new GetTaskTypesQuery(organizationId, page, pageSize, projectId, searchString, sortBy, sortDirection)));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<TaskTypeDto>> GetTaskType(string organizationId, string id, CancellationToken cancellationToken)
    {
        var task = await mediator.Send(new GetTaskTypeQuery(organizationId, id), cancellationToken);

        if (task is null)
        {
            return NotFound();
        }

        return Ok(task);
    }

    [HttpPost]
    [Authorize(Roles = Roles.AdministratorManager)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<TaskTypeDto>> CreateTaskType(string organizationId, CreateTaskTypeDto createTaskTypeDto, CancellationToken cancellationToken)
    {
        try
        {
            var task = await mediator.Send(new CreateTaskTypeCommand(organizationId, createTaskTypeDto.Name, createTaskTypeDto.Description, createTaskTypeDto.ProjectId, createTaskTypeDto.ExcludeHours), cancellationToken);

            return Ok(task);
        }
        catch (Exception)
        {
            return NotFound();
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<TaskTypeDto>> UpdateTaskType(string organizationId, string id, UpdateTaskTypeDto updateTaskTypeDto, CancellationToken cancellationToken)
    {
        try
        {
            var task = await mediator.Send(new UpdateTaskTypeCommand(organizationId, id, updateTaskTypeDto.Name, updateTaskTypeDto.Description, updateTaskTypeDto.ProjectId, updateTaskTypeDto.ExcludeHours), cancellationToken);

            return Ok(task);
        }
        catch (Exception)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.AdministratorManager)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> DeleteTaskType(string organizationId, string id, CancellationToken cancellationToken)
    {
        try
        {
            await mediator.Send(new DeleteTaskTypeCommand(organizationId, id), cancellationToken);

            return Ok();
        }
        catch (Exception)
        {
            return NotFound();
        }
    }
}

public record class CreateTaskTypeDto(string Name, string? Description, string OrganizationId, string? ProjectId, bool ExcludeHours);

public record class UpdateTaskTypeDto(string Name, string? Description, string OrganizationId, string? ProjectId, bool ExcludeHours);
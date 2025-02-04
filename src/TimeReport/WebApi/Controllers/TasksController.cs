using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.ApiKeys;
using YourBrand.TimeReport.Application.Tasks;
using YourBrand.TimeReport.Application.Tasks.Commands;
using YourBrand.TimeReport.Application.Tasks.Queries;
using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Common.Models;

namespace YourBrand.TimeReport.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class TasksController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ItemsResult<TaskDto>>> GetTasks(string organizationId, int page = 0, int pageSize = 10, string? projectId = null, string? userId = null, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new GetTasksQuery(organizationId, page, pageSize, projectId, userId, searchString, sortBy, sortDirection)));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<TaskDto>> GetTask(string organizationId, string id, CancellationToken cancellationToken)
    {
        var task = await mediator.Send(new GetTaskQuery(organizationId, id), cancellationToken);

        if (task is null)
        {
            return NotFound();
        }

        return Ok(task);
    }

    [HttpPost]
    [Authorize(Roles = Roles.AdministratorManager)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<TaskDto>> CreateTask(string organizationId, string projectId, CreateTaskDto createTaskDto, CancellationToken cancellationToken)
    {
        var task = await mediator.Send(new CreateTaskCommand(organizationId, projectId, createTaskDto.Name, createTaskDto.TaskTypeId, createTaskDto.Description, createTaskDto.HourlyRate), cancellationToken);

        return Ok(task);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<TaskDto>> UpdateTask(string organizationId, string id, UpdateTaskDto updateTaskDto, CancellationToken cancellationToken)
    {
        try
        {
            var task = await mediator.Send(new UpdateTaskCommand(organizationId, id, updateTaskDto.Name, updateTaskDto.TaskTypeId, updateTaskDto.Description, updateTaskDto.HourlyRate), cancellationToken);

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
    public async Task<ActionResult> DeleteTask(string organizationId, string id, CancellationToken cancellationToken)
    {
        try
        {
            await mediator.Send(new DeleteTaskCommand(organizationId, id), cancellationToken);

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
            var statistics = await mediator.Send(new GetTaskStatisticsSummaryQuery(organizationId, id), cancellationToken);

            return Ok(mediator.Send(new GetTaskStatisticsSummaryQuery(organizationId, id)));
        }
        catch (Exception)
        {
            return NotFound();
        }
    }
}

public record class CreateTaskDto(string Name, string TaskTypeId, string? Description, decimal? HourlyRate);

public record class UpdateTaskDto(string Name, string TaskTypeId, string? Description, decimal? HourlyRate);
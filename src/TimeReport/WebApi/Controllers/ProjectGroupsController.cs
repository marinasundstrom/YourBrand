using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.ApiKeys;
using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Common.Models;
using YourBrand.TimeReport.Application.Projects.ProjectGroups;
using YourBrand.TimeReport.Application.Projects.ProjectGroups.Commands;
using YourBrand.TimeReport.Application.Projects.ProjectGroups.Queries;

namespace YourBrand.TimeReport.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ProjectGroupsController(IMediator mediator, ITimeReportContext context) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ItemsResult<ProjectGroupDto>>> GetProjectGroups(int page = 0, int pageSize = 10, string? projectId = null, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new GetProjectGroupsQuery(page, pageSize, projectId, searchString, sortBy, sortDirection)));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ProjectGroupDto>> GetProjectGroup(string id, CancellationToken cancellationToken)
    {
        var expense = await mediator.Send(new GetProjectGroupQuery(id), cancellationToken);

        if (expense is null)
        {
            return NotFound();
        }

        return Ok(expense);
    }

    [HttpPost]
    [Authorize(Roles = Roles.AdministratorManager)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ProjectGroupDto>> CreateProjectGroup(CreateProjectGroupDto createProjectGroupDto, CancellationToken cancellationToken)
    {
        try
        {
            var expense = await mediator.Send(new CreateProjectGroupCommand(createProjectGroupDto.Name, createProjectGroupDto.Description), cancellationToken);

            return Ok(expense);
        }
        catch (Exception)
        {
            return NotFound();
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ProjectGroupDto>> UpdateProjectGroup(string id, UpdateProjectGroupDto updateProjectGroupDto, CancellationToken cancellationToken)
    {
        try
        {
            var expense = await mediator.Send(new UpdateProjectGroupCommand(id, updateProjectGroupDto.Name, updateProjectGroupDto.Description), cancellationToken);

            return Ok(expense);
        }
        catch (Exception)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.AdministratorManager)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> DeleteProjectGroup(string id, CancellationToken cancellationToken)
    {
        try
        {
            await mediator.Send(new DeleteProjectGroupCommand(id), cancellationToken);

            return Ok();
        }
        catch (Exception)
        {
            return NotFound();
        }
    }
}

public record class CreateProjectGroupDto(string Name, string? Description);

public record class UpdateProjectGroupDto(string Name, string? Description);
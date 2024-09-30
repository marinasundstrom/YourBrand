using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using YourBrand.Ticketing.Application.Common;
using YourBrand.Ticketing.Application.Features.Projects.ProjectGroups.Commands;
using YourBrand.Ticketing.Application.Features.Projects.ProjectGroups.Queries;

namespace YourBrand.Ticketing.Application.Features.Projects.ProjectGroups;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
[Authorize]
public class ProjectGroupsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ItemsResult<ProjectGroupDto>>> GetProjectGroups(string organizationId, int page = 0, int pageSize = 10, int? projectId = null, string? searchString = null, string? sortBy = null, SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new GetProjectGroupsQuery(organizationId, page, pageSize, projectId, searchString, sortBy, sortDirection)));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ProjectGroupDto>> GetProjectGroup(string organizationId, string id, CancellationToken cancellationToken)
    {
        var expense = await mediator.Send(new GetProjectGroupQuery(organizationId, id), cancellationToken);

        if (expense is null)
        {
            return NotFound();
        }

        return Ok(expense);
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ProjectGroupDto>> CreateProjectGroup(string organizationId, CreateProjectGroupDto createProjectGroupDto, CancellationToken cancellationToken)
    {
        try
        {
            var expense = await mediator.Send(new CreateProjectGroupCommand(organizationId, createProjectGroupDto.Name, createProjectGroupDto.Description), cancellationToken);

            return Ok(expense);
        }
        catch (Exception)
        {
            return NotFound();
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ProjectGroupDto>> UpdateProjectGroup(string organizationId, string id, UpdateProjectGroupDto updateProjectGroupDto, CancellationToken cancellationToken)
    {
        try
        {
            var expense = await mediator.Send(new UpdateProjectGroupCommand(organizationId, id, updateProjectGroupDto.Name, updateProjectGroupDto.Description), cancellationToken);

            return Ok(expense);
        }
        catch (Exception)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> DeleteProjectGroup(string organizationId, string id, CancellationToken cancellationToken)
    {
        try
        {
            await mediator.Send(new DeleteProjectGroupCommand(organizationId, id), cancellationToken);

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
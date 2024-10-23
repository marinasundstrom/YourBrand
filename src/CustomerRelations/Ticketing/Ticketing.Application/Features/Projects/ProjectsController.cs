using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using YourBrand.Ticketing.Application.Common;
using YourBrand.Ticketing.Application.Features.Projects.Commands;
using YourBrand.Ticketing.Application.Features.Projects.Queries;

namespace YourBrand.Ticketing.Application.Features.Projects;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
[Authorize]
public class ProjectsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ItemsResult<ProjectDto>>> GetProjects(string organizationId, int page = 0, int pageSize = 10, string? userId = null, string? searchString = null, string? sortBy = null, SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        var user = Request.HttpContext.User;

        return Ok(await mediator.Send(new GetProjectsQuery(organizationId, page, pageSize, userId, searchString, sortBy, sortDirection), cancellationToken));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ProjectDto>> GetProject(string organizationId, int id, CancellationToken cancellationToken)
    {
        var project = await mediator.Send(new GetProjectQuery(organizationId, id), cancellationToken);

        if (project is null)
        {
            return NotFound();
        }

        return Ok(project);
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ProjectDto>> CreateProject(string organizationId, CreateProjectDto createProjectDto, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateProjectCommand(organizationId, createProjectDto.Name, createProjectDto.Description), cancellationToken);

        return this.HandleResult(result);
    }

    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ProjectDto>> UpdateProject(string organizationId, int id, UpdateProjectDto updateProjectDto, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateProjectCommand(organizationId, id, updateProjectDto.Name, updateProjectDto.Description), cancellationToken);

        return this.HandleResult(result);
    }

    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> DeleteProject(string organizationId, int id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteProjectCommand(organizationId, id), cancellationToken);

        return Ok();
    }

    [HttpGet("{id}/Memberships")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ItemsResult<ProjectMembershipDto>>> GetProjectMemberships(string organizationId, int id, int page = 0, int pageSize = 10, string? sortBy = null, SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetProjectMembershipsQuery(organizationId, id, page, pageSize, sortBy, sortDirection), cancellationToken);

        return this.HandleResult(result);
    }

    [HttpGet("{id}/Memberships/{membershipId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ProjectMembershipDto>> GetProjectMembership(string organizationId, int id, string membershipId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetProjectMembershipQuery(organizationId, id, membershipId), cancellationToken);

        return this.HandleResult(result);
    }

    [HttpPost("{id}/Memberships")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ProjectMembershipDto>> CreateProjectMembership(string organizationId, int id, CreateProjectMembershipDto createProjectMembershipDto, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateProjectMembershipCommand(organizationId, id, createProjectMembershipDto.UserId, createProjectMembershipDto.From, createProjectMembershipDto.Thru), cancellationToken);

        return this.HandleResult(result);
    }

    [HttpPut("{id}/Memberships/{membershipId}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ProjectMembershipDto>> UpdateProjectMembership(string organizationId, int id, string membershipId, UpdateProjectMembershipDto updateProjectMembershipDto, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateProjectMembershipCommand(organizationId, id, membershipId, updateProjectMembershipDto.From, updateProjectMembershipDto.Thru), cancellationToken);

        return this.HandleResult(result);
    }

    [HttpDelete("{id}/Memberships/{membershipId}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> DeleteProjectMembership(string organizationId, int id, string membershipId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteProjectMembershipCommand(organizationId, id, membershipId), cancellationToken);

        return this.HandleResult(result);
    }
}

public record class CreateProjectDto(string Name, string? Description, string? OrganizationId);

public record class UpdateProjectDto(string Name, string? Description, string? OrganizationId);
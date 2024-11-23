using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.ApiKeys;
using YourBrand.TimeReport.Application.Common.Models;
using YourBrand.TimeReport.Application.Invoicing;
using YourBrand.TimeReport.Application.Projects;
using YourBrand.TimeReport.Application.Projects.Commands;
using YourBrand.TimeReport.Application.Projects.Queries;
using YourBrand.TimeReport.Dtos;

namespace YourBrand.TimeReport.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ProjectsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ItemsResult<ProjectDto>>> GetProjects(string organizationId, int page = 0, int pageSize = 10, string? userId = null, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        var user = Request.HttpContext.User;

        return Ok(await mediator.Send(new GetProjectsQuery(organizationId, page, pageSize, userId, searchString, sortBy, sortDirection), cancellationToken));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ProjectDto>> GetProject(string organizationId, string id, CancellationToken cancellationToken)
    {
        var project = await mediator.Send(new GetProjectQuery(organizationId, id), cancellationToken);

        if (project is null)
        {
            return NotFound();
        }

        return Ok(project);
    }

    [HttpPost]
    [Authorize(Roles = Roles.AdministratorManager)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ProjectDto>> CreateProject(string organizationId, CreateProjectDto createProjectDto, CancellationToken cancellationToken)
    {
        var project = await mediator.Send(new CreateProjectCommand(organizationId, createProjectDto.Name, createProjectDto.Description), cancellationToken);

        return this.HandleResult(project);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = Roles.AdministratorManager)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ProjectDto>> UpdateProject(string organizationId, string id, UpdateProjectDto updateProjectDto, CancellationToken cancellationToken)
    {
        var project = await mediator.Send(new UpdateProjectCommand(organizationId, id, updateProjectDto.Name, updateProjectDto.Description), cancellationToken);

        return this.HandleResult(project);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.AdministratorManager)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> DeleteProject(string organizationId, string id, CancellationToken cancellationToken)
    {
        var p = await mediator.Send(new DeleteProjectCommand(organizationId, id), cancellationToken);

        return this.HandleResult(p);

    }

    [HttpGet("Statistics/Summary")]
    public async Task<ActionResult<StatisticsSummary>> GetStatisticsSummary(string organizationId, CancellationToken cancellationToken)
    {
        var r = await mediator.Send(new GetProjectStatisticsSummaryQuery(organizationId), cancellationToken);
        return this.HandleResult(r);
    }

    [HttpGet("Statistics")]
    public async Task<ActionResult<Data>> GetStatistics(string organizationId, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default)
    {
        var r = await mediator.Send(new GetProjectStatisticsQuery(organizationId, from, to), cancellationToken);
        return this.HandleResult(r);
    }

    [HttpGet("{id}/Statistics/Summary")]
    public async Task<ActionResult<StatisticsSummary>> GetStatisticsSummary(string organizationId, string id, CancellationToken cancellationToken)
    {
        var r = await mediator.Send(new GetProjectStatisticsSummaryForProjectQuery(organizationId, id), cancellationToken);
        return this.HandleResult(r);
    }

    [HttpGet("{id}/Statistics")]
    public async Task<ActionResult<Data>> GetProjectStatistics(string organizationId, string id, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default)
    {
        var r = await mediator.Send(new GetProjectStatisticsForProjectQuery(organizationId, id, from, to), cancellationToken);
        return this.HandleResult(r);
    }

    [HttpGet("{id}/Memberships")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ItemsResult<ProjectMembershipDto>>> GetProjectMemberships(string organizationId, string id, int page = 0, int pageSize = 10, string? sortBy = null, TimeReport.Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        var r = await mediator.Send(new GetProjectMembershipsQuery(organizationId, id, page, pageSize, sortBy, sortDirection), cancellationToken);
        return this.HandleResult(r);
    }

    [HttpGet("{id}/Memberships/{membershipId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ProjectMembershipDto>> GetProjectMembership(string organizationId, string id, string membershipId, CancellationToken cancellationToken)
    {
        var r = await mediator.Send(new GetProjectMembershipQuery(organizationId, id, membershipId), cancellationToken);
        return this.HandleResult(r);
    }

    [HttpPost("{id}/Memberships")]
    [Authorize(Roles = Roles.AdministratorManager)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ProjectMembershipDto>> CreateProjectMembership(string organizationId, string id, CreateProjectMembershipDto createProjectMembershipDto, CancellationToken cancellationToken)
    {
        var r = await mediator.Send(new CreateProjectMembershipCommand(organizationId, id, createProjectMembershipDto.UserId, createProjectMembershipDto.From, createProjectMembershipDto.Thru), cancellationToken);
        return this.HandleResult(r);
    }

    [HttpPut("{id}/Memberships/{membershipId}")]
    [Authorize(Roles = Roles.AdministratorManager)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ProjectMembershipDto>> UpdateProjectMembership(string organizationId, string id, string membershipId, UpdateProjectMembershipDto updateProjectMembershipDto, CancellationToken cancellationToken)
    {
        var r = await mediator.Send(new UpdateProjectMembershipCommand(organizationId, id, membershipId, updateProjectMembershipDto.From, updateProjectMembershipDto.Thru), cancellationToken);
        return this.HandleResult(r);
    }

    [HttpDelete("{id}/Memberships/{membershipId}")]
    [Authorize(Roles = Roles.AdministratorManager)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> DeleteProjectMembership(string organizationId, string id, string membershipId, CancellationToken cancellationToken)
    {
        var r = await mediator.Send(new DeleteProjectMembershipCommand(organizationId, id, membershipId), cancellationToken);
        return this.HandleResult(r);
    }

    [HttpPost("{id}/Bill")]
    [Authorize(Roles = Roles.AdministratorManager)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> BillProject(string organizationId, string id, DateTime from, DateTime to, CancellationToken cancellationToken)
    {
        var r = await mediator.Send(new BillProjectCommand(organizationId, id, from, to), cancellationToken);
        return this.HandleResult(r);
    }
}

public record class CreateProjectDto(string Name, string? Description, string? OrganizationId);

public record class UpdateProjectDto(string Name, string? Description, string? OrganizationId);
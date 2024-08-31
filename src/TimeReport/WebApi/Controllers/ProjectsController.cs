using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.ApiKeys;
using YourBrand.TimeReport.Application.Common.Models;
using YourBrand.TimeReport.Application.Invoicing;
using YourBrand.TimeReport.Application.Projects;
using YourBrand.TimeReport.Application.Projects.Commands;
using YourBrand.TimeReport.Application.Projects.Queries;
using YourBrand.TimeReport.Domain.Exceptions;
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
        try
        {
            var project = await mediator.Send(new CreateProjectCommand(organizationId, createProjectDto.Name, createProjectDto.Description), cancellationToken);

            return Ok(project);
        }
        catch (ProjectNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = Roles.AdministratorManager)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ProjectDto>> UpdateProject(string organizationId, string id, UpdateProjectDto updateProjectDto, CancellationToken cancellationToken)
    {
        try
        {
            var project = await mediator.Send(new UpdateProjectCommand(organizationId, id, updateProjectDto.Name, updateProjectDto.Description), cancellationToken);

            return Ok(project);
        }
        catch (ProjectNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.AdministratorManager)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> DeleteProject(string organizationId, string id, CancellationToken cancellationToken)
    {
        try
        {
            await mediator.Send(new DeleteProjectCommand(organizationId, id), cancellationToken);

            return Ok();
        }
        catch (ProjectNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("Statistics/Summary")]
    public async Task<ActionResult<StatisticsSummary>> GetStatisticsSummary(string organizationId, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await mediator.Send(new GetProjectStatisticsSummaryQuery(organizationId), cancellationToken));
        }
        catch (ProjectNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("Statistics")]
    public async Task<ActionResult<Data>> GetStatistics(string organizationId, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default)
    {
        try
        {
            return Ok(await mediator.Send(new GetProjectStatisticsQuery(organizationId, from, to), cancellationToken));
        }
        catch (ProjectNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("{id}/Statistics/Summary")]
    public async Task<ActionResult<StatisticsSummary>> GetStatisticsSummary(string organizationId, string id, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await mediator.Send(new GetProjectStatisticsSummaryForProjectQuery(organizationId, id), cancellationToken));
        }
        catch (ProjectNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("{id}/Statistics")]
    public async Task<ActionResult<Data>> GetProjectStatistics(string organizationId, string id, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default)
    {
        try
        {
            return Ok(await mediator.Send(new GetProjectStatisticsForProjectQuery(organizationId, id, from, to), cancellationToken));
        }
        catch (ProjectNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("{id}/Memberships")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ItemsResult<ProjectMembershipDto>>> GetProjectMemberships(string organizationId, string id, int page = 0, int pageSize = 10, string? sortBy = null, TimeReport.Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        try
        {
            return Ok(await mediator.Send(new GetProjectMembershipsQuery(organizationId, id, page, pageSize, sortBy, sortDirection), cancellationToken));
        }
        catch (ProjectNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("{id}/Memberships/{membershipId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ProjectMembershipDto>> GetProjectMembership(string organizationId, string id, string membershipId, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await mediator.Send(new GetProjectMembershipQuery(organizationId, id, membershipId), cancellationToken));
        }
        catch (ProjectNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("{id}/Memberships")]
    [Authorize(Roles = Roles.AdministratorManager)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ProjectMembershipDto>> CreateProjectMembership(string organizationId, string id, CreateProjectMembershipDto createProjectMembershipDto, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await mediator.Send(new CreateProjectMembershipCommand(organizationId, id, createProjectMembershipDto.UserId, createProjectMembershipDto.From, createProjectMembershipDto.Thru), cancellationToken));
        }
        catch (ProjectNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPut("{id}/Memberships/{membershipId}")]
    [Authorize(Roles = Roles.AdministratorManager)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ProjectMembershipDto>> UpdateProjectMembership(string organizationId, string id, string membershipId, UpdateProjectMembershipDto updateProjectMembershipDto, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await mediator.Send(new UpdateProjectMembershipCommand(organizationId, id, membershipId, updateProjectMembershipDto.From, updateProjectMembershipDto.Thru), cancellationToken));
        }
        catch (ProjectNotFoundException)
        {
            return NotFound();
        }
        catch (ProjectMembershipNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}/Memberships/{membershipId}")]
    [Authorize(Roles = Roles.AdministratorManager)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> DeleteProjectMembership(string organizationId, string id, string membershipId, CancellationToken cancellationToken)
    {
        try
        {
            await mediator.Send(new DeleteProjectMembershipCommand(organizationId, id, membershipId), cancellationToken);

            return Ok();
        }
        catch (ProjectNotFoundException)
        {
            return NotFound();
        }
        catch (ProjectMembershipNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("{id}/Bill")]
    [Authorize(Roles = Roles.AdministratorManager)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> BillProject(string organizationId, string id, DateTime from, DateTime to, CancellationToken cancellationToken)
    {
        try
        {
            await mediator.Send(new BillProjectCommand(organizationId, id, from, to), cancellationToken);

            return Ok();
        }
        catch (ProjectNotFoundException)
        {
            return NotFound();
        }
        catch (ProjectMembershipNotFoundException)
        {
            return NotFound();
        }
    }
}

public record class CreateProjectDto(string Name, string? Description, string? OrganizationId);

public record class UpdateProjectDto(string Name, string? Description, string? OrganizationId);
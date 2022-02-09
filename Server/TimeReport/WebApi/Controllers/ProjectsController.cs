
using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using TimeReport.Application;
using TimeReport.Application.Common.Interfaces;
using TimeReport.Application.Common.Models;
using TimeReport.Application.Projects;
using TimeReport.Application.Projects.Commands;
using TimeReport.Application.Projects.Queries;
using TimeReport.Application.Users;
using TimeReport.Domain.Entities;
using TimeReport.Domain.Exceptions;
using TimeReport.Dtos;

namespace TimeReport.Controllers;

[ApiController]
[Route("[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProjectsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ItemsResult<ProjectDto>>> GetProjects(string? userId = null, int page = 0, int pageSize = 10, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new GetProjectsQuery(page, pageSize, searchString, sortBy, sortDirection), cancellationToken));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ProjectDto>> GetProject(string id, CancellationToken cancellationToken)
    {
        var project = await _mediator.Send(new GetProjectQuery(id), cancellationToken);

        if (project is null)
        {
            return NotFound();
        }

        return Ok(project);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ProjectDto>> CreateProject(CreateProjectDto createProjectDto, CancellationToken cancellationToken)
    {
        try
        {
            var project = await _mediator.Send(new CreateProjectCommand(createProjectDto.Name, createProjectDto.Description), cancellationToken);

            return Ok(project);
        }
        catch (ProjectNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ProjectDto>> UpdateProject(string id, UpdateProjectDto updateProjectDto, CancellationToken cancellationToken)
    {
        try
        {
            var project = await _mediator.Send(new UpdateProjectCommand(id, updateProjectDto.Name, updateProjectDto.Description), cancellationToken);

            return Ok(project);
        }
        catch (ProjectNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> DeleteProject(string id, CancellationToken cancellationToken)
    {
        try
        {
            await _mediator.Send(new DeleteProjectCommand(id), cancellationToken);

            return Ok();
        }
        catch (ProjectNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("Statistics/Summary")]
    public async Task<ActionResult<StatisticsSummary>> GetStatisticsSummary(CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _mediator.Send(new GetProjectStatisticsSummaryQuery(), cancellationToken));
        }
        catch (ProjectNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("Statistics")]
    public async Task<ActionResult<Data>> GetStatistics(DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default)
    {
        try
        {
            return Ok(await _mediator.Send(new GetProjectStatisticsQuery(from, to), cancellationToken));
        }
        catch (ProjectNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("{id}/Statistics/Summary")]
    public async Task<ActionResult<StatisticsSummary>> GetStatisticsSummary(string id, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _mediator.Send(new GetProjectStatisticsSummaryForProjectQuery(id), cancellationToken));
        }
        catch (ProjectNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("{id}/Statistics")]
    public async Task<ActionResult<Data>> GetProjectStatistics(string id, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default)
    {
        try
        {
            return Ok(await _mediator.Send(new GetProjectStatisticsForProjectQuery(id, from, to), cancellationToken));
        }
        catch (ProjectNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("{id}/Memberships")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ItemsResult<ProjectMembershipDto>>> GetProjectMemberships(string id, int page = 0, int pageSize = 10, string? sortBy = null, TimeReport.Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        try
        {
            return Ok(await _mediator.Send(new GetProjectMembershipsQuery(id, page, pageSize, sortBy, sortDirection), cancellationToken));
        }
        catch (ProjectNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("{id}/Memberships/{membershipId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ProjectMembershipDto>> GetProjectMembership(string id, string membershipId, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _mediator.Send(new GetProjectMembershipQuery(id, membershipId), cancellationToken));
        }
        catch (ProjectNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("{id}/Memberships")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ProjectMembershipDto>> CreateProjectMembership(string id, CreateProjectMembershipDto createProjectMembershipDto, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _mediator.Send(new CreateProjectMembershipCommand(id, createProjectMembershipDto.UserId, createProjectMembershipDto.From, createProjectMembershipDto.Thru), cancellationToken));
        }
        catch (ProjectNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPut("{id}/Memberships/{membershipId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ProjectMembershipDto>> UpdateProjectMembership(string id, string membershipId, UpdateProjectMembershipDto updateProjectMembershipDto, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _mediator.Send(new UpdateProjectMembershipCommand(id, membershipId, updateProjectMembershipDto.From, updateProjectMembershipDto.Thru), cancellationToken));
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> DeleteProjectMembership(string id, string membershipId, CancellationToken cancellationToken)
    {
        try
        {
            await _mediator.Send(new DeleteProjectMembershipCommand(id, membershipId), cancellationToken);

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

public record class CreateProjectDto(string Name, string? Description);

public record class UpdateProjectDto(string Name, string? Description);

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourCompany.Showroom.Application.Common.Models;
using YourCompany.Showroom.Application.Skills;
using YourCompany.Showroom.Application.Skills.SkillAreas.Commands;
using YourCompany.Showroom.Application.Skills.SkillAreas.Queries;

namespace YourCompany.Showroom.WebApi.Controllers;

[Authorize]
[Route("[controller]")]
[ApiController]
public class SkillAreasController : ControllerBase
{
    private readonly IMediator _mediator;

    public SkillAreasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<Results<SkillAreaDto>> GetSkillAreas(int page = 1, int pageSize = 10, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetSkillAreasQuery(page - 1, pageSize, searchString, sortBy, sortDirection), cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<SkillAreaDto?> GetSkillArea(string id, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetSkillAreaQuery(id), cancellationToken);
    }

    [HttpPost]
    public async Task CreateSkillArea(CreateSkillAreaDto dto, CancellationToken cancellationToken)
    {
        await _mediator.Send(new CreateSkillAreaCommand(dto.Name), cancellationToken);
    }

    [HttpPut("{id}")]
    public async Task UpdateSkillArea(string id, UpdateSkillAreaDto dto, CancellationToken cancellationToken)
    {
        await _mediator.Send(new UpdateSkillAreaCommand(id, dto.Name), cancellationToken);
    }

    [HttpDelete("{id}")]
    public async Task DeleteSkillArea(string id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteSkillAreaCommand(id), cancellationToken);
    }
}

public record CreateSkillAreaDto(string Name);

public record UpdateSkillAreaDto(string Name);


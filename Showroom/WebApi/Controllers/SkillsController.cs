
using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.ApiKeys;
using YourBrand.Showroom.Application.Common.Models;
using YourBrand.Showroom.Application.Skills;
using YourBrand.Showroom.Application.Skills.Commands;
using YourBrand.Showroom.Application.Skills.Queries;

namespace YourBrand.Showroom.WebApi.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = AuthSchemes.Default)]
public class SkillsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SkillsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<Results<SkillDto>> GetSkills(int page = 1, int pageSize = 10, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetSkillsQuery(page - 1, pageSize, searchString, sortBy, sortDirection), cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<SkillDto?> GetSkill(string id, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetSkillQuery(id), cancellationToken);
    }

    [HttpPost]
    public async Task CreateSkill(CreateSkillDto dto, CancellationToken cancellationToken)
    {
        await _mediator.Send(new CreateSkillCommand(dto.Name), cancellationToken);
    }

    [HttpPut("{id}")]
    public async Task UpdateSkill(string id, UpdateSkillDto dto, CancellationToken cancellationToken)
    {
        await _mediator.Send(new UpdateSkillCommand(id, dto.Name), cancellationToken);
    }

    [HttpDelete("{id}")]
    public async Task DeleteSkill(string id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteSkillCommand(id), cancellationToken);
    }
}

public record CreateSkillDto(string Name, string SkillAreaId);

public record UpdateSkillDto(string Name, string SkillAreaId);


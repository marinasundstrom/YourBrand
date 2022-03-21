
using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourCompany.Showroom.Application.Common.Models;
using YourCompany.Showroom.Application.CompetenceAreas;
using YourCompany.Showroom.Application.CompetenceAreas.Commands;
using YourCompany.Showroom.Application.CompetenceAreas.Queries;
using YourCompany.Showroom.Application.ConsultantProfiles.Queries;

namespace YourCompany.Showroom.WebApi.Controllers;

[Authorize]
[Route("[controller]")]
[ApiController]
public class CompetenceAreasController : ControllerBase
{
    private readonly IMediator _mediator;

    public CompetenceAreasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<Results<CompetenceAreaDto>> GetCompetenceAreas(int page = 1, int pageSize = 10, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetCompetenceAreasQuery(page - 1, pageSize, searchString, sortBy, sortDirection), cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<CompetenceAreaDto?> GetCompetenceArea(string id, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetCompetenceAreaQuery(id), cancellationToken);
    }

    [HttpPost]
    public async Task CreateCompetenceArea(CreateCompetenceAreaDto dto, CancellationToken cancellationToken)
    {
        await _mediator.Send(new CreateCompetenceAreaCommand(dto.Name), cancellationToken);
    }

    [HttpPut("{id}")]
    public async Task UpdateCompetenceArea(string id, UpdateCompetenceAreaDto dto, CancellationToken cancellationToken)
    {
        await _mediator.Send(new UpdateCompetenceAreaCommand(id, dto.Name), cancellationToken);
    }

    [HttpDelete("{id}")]
    public async Task DeleteCompetenceArea(string id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteCompetenceAreaCommand(id), cancellationToken);
    }
}

public record CreateCompetenceAreaDto(string Name);

public record UpdateCompetenceAreaDto(string Name);


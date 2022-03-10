
using MediatR;

using Microsoft.AspNetCore.Mvc;

using Skynet.Showroom.Application.Common.Models;
using Skynet.Showroom.Application.ConsultantProfiles;
using Skynet.Showroom.Application.ConsultantProfiles.Commands;
using Skynet.Showroom.Application.ConsultantProfiles.Queries;

namespace Skynet.Showroom.WebApi.Controllers;

[Route("[controller]")]
[ApiController]
public class ConsultantsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ConsultantsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<Results<ConsultantProfileDto>> GetConsultants(int page = 1, int pageSize = 10, string? organizationId = null, string? competenceAreaId = null, DateTime? availableFrom = null, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetConsultantProfilesQuery(page - 1, pageSize, organizationId, competenceAreaId, availableFrom, searchString, sortBy, sortDirection), cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<ConsultantProfileDto?> GetConsultant(string id, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetConsultantProfileQuery(id), cancellationToken);
    }

    [HttpPost]
    public async Task CreateConsultant(AddConsultantProfileDto dto, CancellationToken cancellationToken)
    {
        await _mediator.Send(new CreateConsultantProfileCommand(dto), cancellationToken);
    }

    [HttpPut("{id}")]
    public async Task UpdateConsultant(string id, UpdateConsultantProfileDto dto, CancellationToken cancellationToken)
    {
        await _mediator.Send(new UpdateConsultantProfileCommand(id, dto), cancellationToken);
    }

    [HttpDelete("{id}")]
    public async Task DeleteConsultant(string id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteConsultantProfileCommand(id), cancellationToken);
    }
}


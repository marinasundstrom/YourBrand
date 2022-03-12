
using MediatR;

using Microsoft.AspNetCore.Mvc;

using Skynet.Showroom.Application.Common.Models;
using Skynet.Showroom.Application.ConsultantProfiles;
using Skynet.Showroom.Application.ConsultantProfiles.Commands;
using Skynet.Showroom.Application.ConsultantProfiles.Experiences;
using Skynet.Showroom.Application.ConsultantProfiles.Experiences.Commands;
using Skynet.Showroom.Application.ConsultantProfiles.Experiences.Queries;
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
    public async Task CreateConsultant(CreateConsultantProfileDto dto, CancellationToken cancellationToken)
    {
        await _mediator.Send(new CreateConsultantProfileCommand(dto), cancellationToken);
    }

    [HttpPut("{id}")]
    public async Task UpdateConsultant(string id, UpdateConsultantProfileDto dto, CancellationToken cancellationToken)
    {
        await _mediator.Send(new UpdateConsultantProfileCommand(id, dto), cancellationToken);
    }

    [HttpPut("{id}/Headline")]
    public async Task UpdateHeadline(string id, [FromBody] string text, CancellationToken cancellationToken)
    {
        await _mediator.Send(new UpdateHeadlineCommand(id, text), cancellationToken);
    }

    [HttpPut("{id}/Presentation")]
    public async Task UpdatePresentation(string id, [FromBody] string text, CancellationToken cancellationToken)
    {
        await _mediator.Send(new UpdatePresentationCommand(id, text), cancellationToken);
    }

    [HttpPut("{id}/Picture")]
    public async Task UpdatePicture(string id, IFormFile file, CancellationToken cancellationToken)
    {
        await _mediator.Send(new UploadImageCommand(id, file.OpenReadStream()), cancellationToken);
    }

    [HttpDelete("{id}")]
    public async Task DeleteConsultant(string id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteConsultantProfileCommand(id), cancellationToken);
    }

    [HttpGet("{id}/Experiences")]
    public async Task<Results<ExperienceDto>> GetExperiences(string? id, int page = 1, int pageSize = 10, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetExperiencesQuery(page - 1, pageSize, id, searchString, sortBy, sortDirection), cancellationToken);
    }

    [HttpGet("{id}/Experiences/{experienceId}")]
    public async Task<ExperienceDto?> GetExperience(string id, string experienceId, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetExperienceQuery(id, experienceId), cancellationToken);
    }

    [HttpPost("{id}/Experiences")]
    public async Task AddExperience(string id, CreateExperienceDto dto, CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new AddExperienceCommand(id, dto.Title, dto.CompanyName, dto.Location, dto.StartDate, dto.EndDate, dto.Description),
            cancellationToken);
    }

    [HttpPut("{id}/Experiences/{experienceId}")]
    public async Task UpdateExperience(string id, string experienceId, UpdateExperienceDto dto, CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new UpdateExperienceCommand(id, experienceId,
            dto.Title, dto.CompanyName, dto.Location, dto.StartDate, dto.EndDate, dto.Description),
            cancellationToken);
    }

    [HttpDelete("{id}/Experiences/{experienceId}")]
    public async Task RemoveExperience(string id, string experienceId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new RemoveExperienceCommand(id, experienceId), cancellationToken);
    }
}


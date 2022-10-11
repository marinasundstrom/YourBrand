
using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.ApiKeys;
using YourBrand.Showroom.Application.Common.Models;
using YourBrand.Showroom.Application.Industries;
using YourBrand.Showroom.Application.Industries.Commands;
using YourBrand.Showroom.Application.Industries.Queries;
using YourBrand.Showroom.Application.PersonProfiles.Queries;

namespace YourBrand.Showroom.WebApi.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = AuthSchemes.Default)]
public class IndustriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public IndustriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<Results<IndustryDto>> GetIndustries(int page = 1, int pageSize = 10, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetIndustriesQuery(page - 1, pageSize, searchString, sortBy, sortDirection), cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<IndustryDto?> GetIndustry(int id, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetIndustryQuery(id), cancellationToken);
    }

    [HttpPost]
    public async Task<IndustryDto> CreateIndustry(CreateIndustryDto dto, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new CreateIndustryCommand(dto.Name), cancellationToken);
    }

    [HttpPut("{id}")]
    public async Task UpdateIndustry(int id, UpdateIndustryDto dto, CancellationToken cancellationToken)
    {
        await _mediator.Send(new UpdateIndustryCommand(id, dto.Name), cancellationToken);
    }

    [HttpDelete("{id}")]
    public async Task DeleteIndustry(int id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteIndustryCommand(id), cancellationToken);
    }
}

public record CreateIndustryDto(string Name);

public record UpdateIndustryDto(string Name);


using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using YourBrand.Marketing.Application.Campaigns.Commands;
using YourBrand.Marketing.Application.Campaigns.Queries;

namespace YourBrand.Marketing.Application.Campaigns;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
public class CampaignsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CampaignsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ItemsResult<CampaignDto>> GetCampaigns(int page = 1, int pageSize = 10, string? groupId = null, string? warehouseId = null, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetCampaignsQuery(page, pageSize, searchString, sortBy, sortDirection), cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<CampaignDto?> GetCampaign(string id, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetCampaignQuery(id), cancellationToken);
    }

    [HttpPost]
    public async Task<CampaignDto> CreateCampaign(CreateCampaignDto dto, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new CreateCampaignCommand(dto.Name), cancellationToken);
    }

    [HttpPut("{id}")]
    public async Task UpdateCampaign(string id, UpdateCampaignDto dto, CancellationToken cancellationToken)
    {
        await _mediator.Send(new UpdateCampaignCommand(id, dto.Name), cancellationToken);
    }

    [HttpDelete("{id}")]
    public async Task DeleteCampaign(string id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteCampaignCommand(id), cancellationToken);
    }
}

public record CreateCampaignDto(string Name);

public record UpdateCampaignDto(string Name);
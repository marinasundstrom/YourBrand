using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using YourBrand.Inventory.Application.Sites.Commands;
using YourBrand.Inventory.Application.Sites.Queries;

namespace YourBrand.Inventory.Application.Sites;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
public class SitesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SitesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ItemsResult<SiteDto>> GetSites(int page = 1, int pageSize = 10, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetSitesQuery(page - 1, pageSize, searchString, sortBy, sortDirection), cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<SiteDto?> GetSite(string id, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetSiteQuery(id), cancellationToken);
    }

    [HttpPost]
    public async Task<SiteDto> CreateSite(CreateSiteDto dto, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new CreateSiteCommand(dto.Name, dto.CreateWarehouse), cancellationToken);
    }

    [HttpPut("{id}")]
    public async Task UpdateSite(string id, UpdateSiteDto dto, CancellationToken cancellationToken)
    {
        await _mediator.Send(new UpdateSiteCommand(id, dto.Name), cancellationToken);
    }

    [HttpDelete("{id}")]
    public async Task DeleteSite(string id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteSiteCommand(id), cancellationToken);
    }
}

public record CreateSiteDto(string Name, bool CreateWarehouse);

public record UpdateSiteDto(string Name);
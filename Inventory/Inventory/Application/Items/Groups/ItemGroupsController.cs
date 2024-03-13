using YourBrand.Inventory.Application;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using YourBrand.Inventory.Application.Items.Groups.Queries;
using Microsoft.AspNetCore.Authorization;
using YourBrand.Inventory.Application.Items.Groups;
using YourBrand.Inventory.Application.Items.Groups.Commands;
using YourBrand.Inventory.Application.Common.Models;
using Asp.Versioning;

namespace YourBrand.Inventory.Application.Items.Groups;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
public class GroupsController : ControllerBase
{
    private readonly IMediator _mediator;

    public GroupsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ItemsResult<ItemGroupDto>> GetGroups(int page = 1, int pageSize = 10, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetItemGroups(page - 1, pageSize, null, searchString, sortBy, sortDirection), cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<ItemGroupDto?> GetGroup(string id, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetItemGroup(id), cancellationToken);
    }

    [HttpPost]
    public async Task<ItemGroupDto> CreateGroup(CreateGroupDto dto, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new CreateItemGroup(dto.Name), cancellationToken);
    }

    [HttpPut("{id}")]
    public async Task UpdateGroup(string id, UpdateGroupDto dto, CancellationToken cancellationToken)
    {
        await _mediator.Send(new UpdateItemGroup(id, dto.Name), cancellationToken);
    }

    [HttpDelete("{id}")]
    public async Task DeleteGroup(string id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteItemGroup(id), cancellationToken);
    }
}

public record CreateGroupDto(string Name);

public record UpdateGroupDto(string Name);


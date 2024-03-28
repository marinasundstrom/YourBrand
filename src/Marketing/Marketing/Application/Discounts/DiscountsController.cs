using YourBrand.Marketing.Application;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using YourBrand.Marketing.Application.Discounts;
using YourBrand.Marketing.Application.Discounts.Queries;
using YourBrand.Marketing.Application.Discounts.Commands;
using YourBrand.Marketing.Application.Common.Models;
using Asp.Versioning;

namespace YourBrand.Marketing.Application.Discounts;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
public class DiscountsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DiscountsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ItemsResult<DiscountDto>> GetDiscounts(int page = 1, int pageSize = 10, string? groupId = null, string? warehouseId = null, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetDiscountsQuery(page, pageSize, searchString, sortBy, sortDirection), cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<DiscountDto?> GetDiscount(string id, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetDiscountQuery(id), cancellationToken);
    }

    [HttpPost]
    public async Task<DiscountDto> CreateDiscount(CreateDiscountDto dto, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new CreateDiscountCommand(dto.Percentage, dto.Amount), cancellationToken);
    }

    [HttpPut("{id}")]
    public async Task UpdateDiscount(string id, UpdateDiscountDto dto, CancellationToken cancellationToken)
    {
        await _mediator.Send(new UpdateDiscountCommand(id, dto.ItemId, dto.ItemName, dto.ItemDescription, dto.Percentage, dto.Amount), cancellationToken);
    }

    [HttpDelete("{id}")]
    public async Task DeleteDiscount(string id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteDiscountCommand(id), cancellationToken);
    }
}

public record CreateDiscountDto(
                double Percentage,
                decimal Amount);

public record UpdateDiscountDto(
                string ItemId,
                string ItemName,
                string ItemDescription,
                double Percentage,
                decimal Amount);

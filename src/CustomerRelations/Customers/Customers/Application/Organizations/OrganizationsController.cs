using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using YourBrand.Customers.Application.Addresses;
using YourBrand.Customers.Application.Commands;
using YourBrand.Customers.Application.Organizations.Commands;
using YourBrand.Customers.Application.Organizations.Queries;

namespace YourBrand.Customers.Application.Organizations;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
public class OrganizationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrganizationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<ItemsResult<Organizations.OrganizationDto>>> GetOrganizations(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetOrganizations(page, pageSize), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<OrganizationDto?> GetOrganization(int id, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetOrganization(id), cancellationToken);
    }

    [HttpPost]
    [ProducesDefaultResponseType]
    [ProducesResponseType(typeof(OrganizationDto), StatusCodes.Status201Created)]
    public async Task<ActionResult> CreateOrganization([FromBody] CreateOrganizationDto dto, CancellationToken cancellationToken)
    {
        var dto2 = await _mediator.Send(new CreateOrganization(dto.Name, dto.OrgNo, dto.Phone, dto.PhoneMobile, dto.Email, dto.Address), cancellationToken);
        return CreatedAtAction(nameof(GetOrganization), new { id = dto2.Id }, dto2);
    }

    [HttpPut("{id}")]
    public async Task UpdateOrganization(string id, [FromBody] UpdateOrganizationDto dto, CancellationToken cancellationToken)
    {
        //await _mediator.Send(new UpdateOrganization(idto.Name, dto.OrgNo), cancellationToken);
    }

    [HttpDelete("{id}")]
    public async Task DeleteOrganization(string id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteOrganization(id), cancellationToken);
    }
}


public record CreateOrganizationDto(string Name, string OrgNo, string? Phone, string? PhoneMobile, string? Email, Address2Dto Address);

public record UpdateOrganizationDto(string Name, string OrgNo, string? Phone, string? PhoneMobile, string? Email);

using AspNetCore.Authentication.ApiKey;

using MediatR;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.HumanResources.Application.Common.Models;
using YourBrand.HumanResources.Application.Organizations;
using YourBrand.HumanResources.Application.Organizations.Commands;
using YourBrand.HumanResources.Application.Organizations.Queries;
using YourBrand.HumanResources.Domain.Exceptions;

namespace YourBrand.HumanResources;

[Route("[controller]")]
[ApiController]
[Authorize]
public class OrganizationsController : Controller
{
    private readonly IMediator _mediator;

    public OrganizationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ItemsResult<OrganizationDto>>> GetOrganizations(int page = 0, int pageSize = 10, string? searchString = null, string? sortBy = null, HumanResources.Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new GetOrganizationsQuery(page, pageSize, searchString, sortBy, sortDirection), cancellationToken));

    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<OrganizationDto>> GetOrganization(string id, CancellationToken cancellationToken)
    {
        var organization = await _mediator.Send(new GetOrganizationQuery(id), cancellationToken);

        if (organization is null)
        {
            return NotFound();
        }

        return Ok(organization);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<OrganizationDto>> CreateOrganization(CreateOrganizationDto createOrganizationDto, CancellationToken cancellationToken)
    {
        try
        {
            var organization = await _mediator.Send(new CreateOrganizationCommand(createOrganizationDto.Name, createOrganizationDto.FriendlyName), cancellationToken);

            return Ok(organization);
        }
        catch (Exception)
        {
            throw;
        }
    }

    [HttpPut("{id}/Details")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<OrganizationDto>> UpdateOrganization(string id, UpdateOrganizationDto updateOrganizationDto, CancellationToken cancellationToken)
    {
        try
        {
            var organization = await _mediator.Send(new UpdateOrganizationCommand(id, updateOrganizationDto.Name), cancellationToken);

            return Ok(organization);
        }
        catch (OrganizationNotFoundException)
        {
            return NotFound();
        }
    }
    
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> DeleteOrganization(string id, CancellationToken cancellationToken)
    {
        try
        {
            await _mediator.Send(new DeleteOrganizationCommand(id), cancellationToken);

            return Ok();
        }
        catch (OrganizationNotFoundException)
        {
            return NotFound();
        }
    }
}

public record class CreateOrganizationDto(string Name, string? FriendlyName);

public record class UpdateOrganizationDto(string Name);
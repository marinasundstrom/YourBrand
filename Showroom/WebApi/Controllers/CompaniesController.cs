
using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.Showroom.Application.Common.Models;
using YourBrand.Showroom.Application.Companies;
using YourBrand.ApiKeys;

namespace YourBrand.Showroom.WebApi.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = AuthSchemes.Default)]
public class CompaniesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CompaniesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<Results<CompanyDto>> GetCompanies(int page = 1, int pageSize = 10, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetCompaniesQuery(page - 1, pageSize, searchString, sortBy, sortDirection), cancellationToken);
    }

    /*

    [HttpGet("{id}")]
    public async Task<CompanyDto?> GetCompany(string id, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetCompanyQuery(id), cancellationToken);
    }

    [HttpPost]
    [ProducesDefaultResponseType]
    [ProducesResponseType(typeof(CompanyDto), StatusCodes.Status201Created)]
    public async Task<ActionResult> CreateCompany(CreateCompanyDto dto, CancellationToken cancellationToken)
    {
        var dto2 = await _mediator.Send(new CreateCompanyCommand(dto.Description), cancellationToken);
        return CreatedAtAction(nameof(GetCompany), new { id = dto2.Id }, dto2);
    }

    [HttpPut("{id}")]
    public async Task UpdateCompany(string id, UpdateCompanyDto dto, CancellationToken cancellationToken)
    {
        await _mediator.Send(new UpdateCompanyCommand(id, dto.Description), cancellationToken);
    }

    [HttpDelete("{id}")]
    public async Task DeleteCompany(string id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteCompanyCommand(id), cancellationToken);
    }

    */
}



using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.ApiKeys;
using YourBrand.Showroom.Application.Common.Models;
using YourBrand.Showroom.Application.Companies;
using YourBrand.Showroom.Application.Companies.Commands;
using YourBrand.Showroom.Application.Companies.Queries;

namespace YourBrand.Showroom.WebApi.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = AuthSchemes.Default)]
public class CompaniesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<Results<CompanyDto>> GetCompanies(int page = 1, int pageSize = 10, int? industryId = null, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return await mediator.Send(new GetCompaniesQuery(page - 1, pageSize, industryId, searchString, sortBy, sortDirection), cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<CompanyDto?> GetCompany(string id, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetCompanyQuery(id), cancellationToken);
    }

    [HttpPost]
    public async Task<CompanyDto> CreateCompany(CreateCompanyDto dto, CancellationToken cancellationToken)
    {
        return await mediator.Send(new CreateCompanyCommand(dto.Name, dto.IndustryId), cancellationToken);
    }

    [HttpPut("{id}")]
    public async Task UpdateCompany(string id, UpdateCompanyDto dto, CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdateCompanyCommand(id, dto.Name, dto.IndustryId), cancellationToken);
    }

    [HttpDelete("{id}")]
    public async Task DeleteCompany(string id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteCompanyCommand(id), cancellationToken);
    }
}

public record CreateCompanyDto(string Name, int IndustryId);

public record UpdateCompanyDto(string Name, int IndustryId);
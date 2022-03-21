
using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourCompany.Showroom.Application.Common.Models;
using YourCompany.Showroom.Application.Cases;
using YourCompany.Showroom.Application.Cases.Commands;
using YourCompany.Showroom.Application.Cases.Queries;

namespace YourCompany.Showroom.WebApi.Controllers;

[Authorize]
[Route("[controller]")]
[ApiController]
public class CasesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CasesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<Results<CaseDto>> GetCases(int page = 1, int pageSize = 10, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetCasesQuery(page - 1, pageSize, searchString, sortBy, sortDirection), cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<CaseDto?> GetCase(string id, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetCaseQuery(id), cancellationToken);
    }

    [HttpPost]
    public async Task CreateCase(CreateCaseDto dto, CancellationToken cancellationToken)
    {
        await _mediator.Send(new CreateCaseCommand(dto.Description), cancellationToken);
    }

    [HttpPut("{id}")]
    public async Task UpdateCase(string id, UpdateCaseDto dto, CancellationToken cancellationToken)
    {
        await _mediator.Send(new UpdateCaseCommand(id, dto.Description), cancellationToken);
    }

    [HttpDelete("{id}")]
    public async Task DeleteCase(string id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteCaseCommand(id), cancellationToken);
    }
}

public record CreateCaseDto(string? Description);

public record UpdateCaseDto(string? Description);


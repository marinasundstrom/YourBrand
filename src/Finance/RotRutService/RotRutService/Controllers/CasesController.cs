using MediatR;

using Microsoft.AspNetCore.Mvc;

using YourBrand.RotRutService.Application;
using YourBrand.RotRutService.Application.Queries;
using YourBrand.RotRutService.Domain.Entities;
using YourBrand.RotRutService.Domain.Enums;

namespace YourBrand.RotRutService.Controllers;

[Route("[controller]")]
public class CasesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ItemsResult<RotRutCaseDto>>> GetCases(int page, int pageSize, [FromQuery] DomesticServiceKind? kind, [FromQuery] RotRutCaseStatus[]? status, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetCases(page, pageSize, kind, status), cancellationToken);
        return Ok(result);
    }
}
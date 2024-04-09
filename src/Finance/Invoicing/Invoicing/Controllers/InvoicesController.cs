using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using YourBrand.Invoicing.Application;
using YourBrand.Invoicing.Application.Queries;
using YourBrand.Invoicing.Domain.Enums;

namespace YourBrand.Invoicing.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
public class InvoicesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ItemsResult<InvoiceDto>>> GetInvoicesAsync(int page, int pageSize, [FromQuery] InvoiceType[]? type, [FromQuery] InvoiceStatus[]? status, [FromQuery] string? reference, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetInvoices(page, pageSize, type, status, reference), cancellationToken);
        return Ok(result);
    }
}
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
    public async Task<ActionResult<ItemsResult<InvoiceDto>>> GetInvoicesAsync(string organizationId, string? customerId = null, int page = 1, int pageSize = 10, [FromQuery] InvoiceType[]? type = null, [FromQuery] int[]? status = null, [FromQuery] string? reference = null, string? sortBy = null, SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetInvoices(organizationId, customerId, page, pageSize, type, status, reference, sortBy, sortDirection), cancellationToken);
        return Ok(result);
    }
}
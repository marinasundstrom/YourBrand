using YourBrand.Invoicing.Application;
using YourBrand.Invoicing.Domain.Enums;
using YourBrand.Invoicing.Application.Queries;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace YourBrand.Invoicing.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
public class InvoicesController : ControllerBase 
{
    private readonly IMediator _mediator;

    public InvoicesController(IMediator mediator) 
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<ItemsResult<InvoiceDto>>> GetInvoicesAsync(int page, int pageSize, [FromQuery] InvoiceType[]? type, [FromQuery] InvoiceStatus[]? status, [FromQuery] string? reference, CancellationToken cancellationToken = default) 
    {
        var result = await _mediator.Send(new GetInvoices(page, pageSize, type, status, reference), cancellationToken);
        return Ok(result);
    }
}
using YourBrand.RotRutService.Application;
using YourBrand.RotRutService.Domain.Enums;
using YourBrand.RotRutService.Application.Queries;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace YourBrand.RotRutService.Controllers;

[Route("[controller]")]
public class InvoicesController : ControllerBase 
{
    private readonly IMediator _mediator;

    public InvoicesController(IMediator mediator) 
    {
        _mediator = mediator;
    }

    /*
    [HttpGet]
    public async Task<ActionResult<ItemsResult<InvoiceDto>>> GetInvoicesAsync(int page, int pageSize, [FromQuery] InvoiceType[]? type, [FromQuery] InvoiceStatus[]? status, [FromQuery] string? reference, CancellationToken cancellationToken = default) 
    {
        var result = await _mediator.Send(new GetInvoices(page, pageSize, type, status, reference), cancellationToken);
        return Ok(result);
    }
    */
}
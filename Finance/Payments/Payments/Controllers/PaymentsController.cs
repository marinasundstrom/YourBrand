using YourBrand.Payments.Application;
using YourBrand.Payments.Domain.Enums;
using YourBrand.Payments.Application.Queries;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Invoices.Controllers;

[Route("[controller]")]
public class PaymentsController : ControllerBase 
{
    private readonly IMediator _mediator;

    public PaymentsController(IMediator mediator) 
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<ItemsResult<PaymentDto>>> GetPaymentsAsync(int page, int pageSize, [FromQuery] PaymentStatus[]? status,  [FromQuery] int? invoiceId, CancellationToken cancellationToken = default) 
    {
        var result = await _mediator.Send(new GetPayments(page, pageSize, status, invoiceId), cancellationToken);
        return Ok(result);
    }
}
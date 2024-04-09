using MediatR;

using Microsoft.AspNetCore.Mvc;

using YourBrand.Payments.Application;
using YourBrand.Payments.Application.Queries;
using YourBrand.Payments.Domain.Enums;

namespace Invoices.Controllers;

[Route("[controller]")]
public class PaymentsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ItemsResult<PaymentDto>>> GetPaymentsAsync(int page, int pageSize, [FromQuery] PaymentStatus[]? status, [FromQuery] string? invoiceId, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetPayments(page, pageSize, status, invoiceId), cancellationToken);
        return Ok(result);
    }
}
using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.Payments.Application;
using YourBrand.Payments.Application.Queries;
using YourBrand.Payments.Domain.Enums;

namespace Invoices.Controllers;

[ApiController]
[Route("v{version:apiVersion}/[controller]")]
[Authorize]
[ApiVersion("1")]
public class PaymentsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ItemsResult<PaymentDto>>> GetPaymentsAsync(string organizationId, int page, int pageSize, [FromQuery] PaymentStatus[]? status, [FromQuery] string? invoiceId, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetPayments(organizationId, page, pageSize, status, invoiceId), cancellationToken);
        return Ok(result);
    }
}
using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using YourBrand.Transactions.Application;
using YourBrand.Transactions.Application.Queries;
using YourBrand.Transactions.Domain.Enums;

namespace YourBrand.Invoicing.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
public class TransactionsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ItemsResult<TransactionDto>>> GetTransactionsAsync(int page, int pageSize, [FromQuery] TransactionStatus[]? status, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetTransactions(page, pageSize, status), cancellationToken);
        return Ok(result);
    }
}
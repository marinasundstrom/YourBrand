using YourBrand.Transactions.Application;
using YourBrand.Transactions.Domain.Enums;
using YourBrand.Transactions.Application.Queries;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace YourBrand.Invoicing.Controllers;

[Route("[controller]")]
public class TransactionsController : ControllerBase 
{
    private readonly IMediator _mediator;

    public TransactionsController(IMediator mediator) 
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<ItemsResult<TransactionDto>>> GetTransactionsAsync(int page, int pageSize, [FromQuery] TransactionStatus[]? status, CancellationToken cancellationToken = default) 
    {
        var result = await _mediator.Send(new GetTransactions(page, pageSize, status), cancellationToken);
        return Ok(result);
    }
}
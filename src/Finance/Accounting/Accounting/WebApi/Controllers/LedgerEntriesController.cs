using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.Accounting.Application.Ledger;
using YourBrand.Accounting.Application.Ledger.Queries;

namespace YourBrand.Accounting.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
[Authorize]
public class LedgerEntriesController(IMediator mediator) : Controller
{
    [HttpGet]
    public async Task<LedgerEntriesResult> GetLedgerEntriesAsync(string organizationId, int? accountNo = null, int? journalEntryId = null, int page = 0, int pageSize = 10, ResultDirection direction = ResultDirection.Asc, CancellationToken cancellationToken = default)
    {
        return await mediator.Send(new GetLedgerEntriesQuery(organizationId, accountNo, journalEntryId, page, pageSize, direction), cancellationToken);
    }
}
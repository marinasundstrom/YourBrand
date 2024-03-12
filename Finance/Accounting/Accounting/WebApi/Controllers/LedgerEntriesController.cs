using YourBrand.Accounting.Application.Ledger;
using YourBrand.Accounting.Application.Ledger.Queries;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace YourBrand.Accounting.Controllers
{
    [Route("[controller]")]
    public class LedgerEntriesController : Controller
    {
        private readonly IMediator mediator;

        public LedgerEntriesController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        public async Task<LedgerEntriesResult> GetLedgerEntriesAsync(int? accountNo = null, int? journalEntryId = null, int page = 0, int pageSize = 10, ResultDirection direction = ResultDirection.Asc, CancellationToken cancellationToken = default)
        {
            return await mediator.Send(new GetLedgerEntriesQuery(accountNo, journalEntryId, page, pageSize, direction), cancellationToken);
        }
    }
}
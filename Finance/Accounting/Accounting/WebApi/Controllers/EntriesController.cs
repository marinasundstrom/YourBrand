using Accounting.Application.Entries;
using Accounting.Application.Entries.Queries;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Accounting.Controllers
{
    [Route("[controller]")]
    public class EntriesController : Controller
    {
        private readonly IMediator mediator;

        public EntriesController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        public async Task<EntriesResult> GetEntriesAsync(int? accountNo = null, int? verificationId = null, int page = 0, int pageSize = 10, ResultDirection direction = ResultDirection.Asc, CancellationToken cancellationToken = default)
        {
            return await mediator.Send(new GetEntriesQuery(accountNo, verificationId, page, pageSize, direction), cancellationToken);
        }
    }
}
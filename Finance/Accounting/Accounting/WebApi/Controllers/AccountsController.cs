using Accounting.Application.Accounts;
using Accounting.Application.Accounts.Queries;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Accounting.Controllers
{
    [Route("[controller]")]
    public class AccountsController : Controller
    {
        private readonly IMediator mediator;

        public AccountsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        public async Task<IEnumerable<AccountDto>> GetAccounts(int? accountClass = null, bool includeBlankAccounts = true, bool includeUnusedAccounts = false, CancellationToken cancellationToken = default)
        {
            return await mediator.Send(new GetAccountsQuery(accountClass, includeBlankAccounts, includeUnusedAccounts), cancellationToken);
        }

        [HttpGet("{accountNo:int}")]
        public async Task<AccountDto> GetAccount(int accountNo, CancellationToken cancellationToken)
        {
            return await mediator.Send(new GetAccountQuery(accountNo), cancellationToken);
        }

        [HttpGet("Classes")]
        public async Task<IEnumerable<AccountClassDto>> GetAccountClasses(CancellationToken cancellationToken)
        {
            return await mediator.Send(new GetAccountClassesQuery(), cancellationToken);
        }

        [HttpGet("Classes/Summary")]
        public async Task<IEnumerable<AccountClassSummary>> GetAccountClassSummary(int[] accountNo, CancellationToken cancellationToken)
        {
            return await mediator.Send(new GetAccountsClassesSummaryQuery(accountNo), cancellationToken);
        }

        [HttpGet("History")]
        public async Task<AccountBalanceHistory> GetAccountHistory(int[] accountNo, CancellationToken cancellationToken)
        {
            return await mediator.Send(new GetAccountHistoryQuery(accountNo), cancellationToken);
        }
    }
}
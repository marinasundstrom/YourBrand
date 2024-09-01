using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.Accounting.Application.Accounts;
using YourBrand.Accounting.Application.Accounts.Commands;
using YourBrand.Accounting.Application.Accounts.Queries;

namespace YourBrand.Accounting.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
[Authorize]
public class AccountsController(IMediator mediator) : Controller
{
    [HttpGet]
    public async Task<IEnumerable<AccountDto>> GetAccounts(string organizationId, int? accountClass = null, bool includeBlankAccounts = true, bool includeUnusedAccounts = false, CancellationToken cancellationToken = default)
    {
        return await mediator.Send(new GetAccountsQuery(organizationId, accountClass, includeBlankAccounts, includeUnusedAccounts), cancellationToken);
    }

    [HttpGet("{accountNo:int}")]
    public async Task<AccountDto> GetAccount(string organizationId, int accountNo, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetAccountQuery(organizationId, accountNo), cancellationToken);
    }

    [HttpGet("Classes")]
    public async Task<IEnumerable<AccountClassDto>> GetAccountClasses(string organizationId, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetAccountClassesQuery(organizationId), cancellationToken);
    }

    [HttpGet("Classes/Summary")]
    public async Task<IEnumerable<AccountClassSummary>> GetAccountClassSummary(string organizationId, [FromQuery] int[] accountNo, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetAccountsClassesSummaryQuery(organizationId, accountNo), cancellationToken);
    }

    [HttpGet("History")]
    public async Task<AccountBalanceHistory> GetAccountHistory(string organizationId, [FromQuery] int[] accountNo, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetAccountHistoryQuery(organizationId, accountNo), cancellationToken);
    }

    [HttpPost]
    public async Task CreateAccounts(string organizationId, CancellationToken cancellationToken)
    {
        await mediator.Send(new CreateAccountsCommand(organizationId), cancellationToken);
    }
}

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Accounting.Application.Common.Interfaces;

using static YourBrand.Accounting.Application.Accounts.Mappings;

namespace YourBrand.Accounting.Application.Accounts.Queries;

public record GetAccountsQuery(
    int? AccountClass = null,
    bool IncludeBlankAccounts = true,
    bool IncludeUnusedAccounts = false)
    : IRequest<IEnumerable<AccountDto>>
{
    public class GetAccountsQueryHandler(IAccountingContext context) : IRequestHandler<GetAccountsQuery, IEnumerable<AccountDto>>
    {
        public async Task<IEnumerable<AccountDto>> Handle(GetAccountsQuery request, CancellationToken cancellationToken)
        {
            var query = context.Accounts
                            .Include(a => a.Entries)
                            .AsNoTracking()
                            .AsQueryable();

            if (!request.IncludeBlankAccounts)
            {
                query = query.Where(a => a.Entries.Select(e => e.Debit.GetValueOrDefault() - e.Credit.GetValueOrDefault()).Sum() != 0);
            }

            if (!request.IncludeUnusedAccounts)
            {
                query = query.Where(a => a.Entries.Any());
            }

            if (request.AccountClass is not null)
            {
                query = query.Where(a => a.Class == (Domain.Enums.AccountClass)request.AccountClass);
            }

            var r = await query.ToListAsync(cancellationToken);

            var vms = new List<AccountDto>();

            vms.AddRange(r.Select(MapAccount));

            return vms;
        }
    }
}
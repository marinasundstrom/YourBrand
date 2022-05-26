using System.ComponentModel.DataAnnotations;

using Accounting.Application.Common.Interfaces;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Accounting.Application.Accounts.Queries
{
    public record GetAccountsClassesSummaryQuery(int[] AccountNo) : IRequest<IEnumerable<AccountClassSummary>>
    {
        public class GetAccountsClassesSummaryQueryHandler : IRequestHandler<GetAccountsClassesSummaryQuery, IEnumerable<AccountClassSummary>>
        {
            private readonly IAccountingContext context;

            public GetAccountsClassesSummaryQueryHandler(IAccountingContext context)
            {
                this.context = context;
            }

            public async Task<IEnumerable<AccountClassSummary>> Handle(GetAccountsClassesSummaryQuery request, CancellationToken cancellationToken)
            {
                var query = context.Accounts
                    .Include(a => a.Entries)
                    .Where(a => a.Entries.Any())
                    .AsNoTracking()
                    .AsQueryable();

                var accounts = await query.ToListAsync(cancellationToken);

                var classes = accounts.GroupBy(x => x.Class);

                return classes.Select(c =>
                {
                    var name = c.Key.GetAttribute<DisplayAttribute>()!.Name!;
                    return new AccountClassSummary(
                        (int)c.Key,
                        name,
                        c.Sum(a => a.Entries.Select(g => g.Debit.GetValueOrDefault() - g.Credit.GetValueOrDefault()).Sum())
                    );
                });
            }
        }
    }
}
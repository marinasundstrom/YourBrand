
using Accounting.Application.Common.Interfaces;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Accounting.Application.Accounts.Queries;

public record GetAccountHistoryQuery(int[] AccountNo) : IRequest<AccountBalanceHistory>
{
    public class GetAccountHistoryQueryHandler : IRequestHandler<GetAccountHistoryQuery, AccountBalanceHistory>
    {
        private readonly IAccountingContext context;

        public GetAccountHistoryQueryHandler(IAccountingContext context)
        {
            this.context = context;
        }

        public async Task<AccountBalanceHistory> Handle(GetAccountHistoryQuery request, CancellationToken cancellationToken)
        {
            List<(int Year, int Month)> months = new();

            DateTime startDate = DateTime.Today.Subtract(TimeSpan.FromDays(365));
            DateTime endDate = DateTime.Today;
            for (DateTime dt = startDate; dt <= endDate; dt = dt.AddMonths(1))
            {
                months.Add((dt.Year, dt.Month));
            }

            var query = context.Accounts
                .Include(a => a.Entries)
                .ThenInclude(e => e.Verification)
                .Where(a => a.Entries.Any())
                .AsNoTracking()
                .AsQueryable();

            if (request.AccountNo.Any())
            {
                query = query.Where(a => request.AccountNo.Any(x => x == a.AccountNo));
            }

            var accounts = await query.ToListAsync(cancellationToken);

            List<string> labels = months.Select(m => new DateOnly(m.Year, m.Month, 1).ToString("MMM yy")).ToList();
            Dictionary<Domain.Entities.Account, List<decimal>> series = new();

            foreach (var month in months)
            {
                foreach (var account in accounts)
                {
                    if (!series.ContainsKey(account))
                    {
                        series[account] = new List<decimal>();
                    }

                    var entries = account.Entries.Where(x => x.Verification.Date.Year == month.Year && x.Verification.Date.Month == month.Month);
                    var sum = entries.Select(g => g.Debit.GetValueOrDefault() - g.Credit.GetValueOrDefault()).Sum();

                    if (sum < 0)
                    {
                        // Assume it is supposed to be a negative value but that we want to represent it as a positve value.
                        sum = sum * -1;
                    }

                    series[account].Add(sum);
                }
            }

            return new AccountBalanceHistory(
                labels.ToArray(),
                series.Select(asum => new AccountSeries(
                    $"{asum.Key.AccountNo} {asum.Key.Name}",
                    asum.Value)));
        }
    }
}
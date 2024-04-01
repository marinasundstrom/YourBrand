using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Accounting.Application.Common.Interfaces;

using static YourBrand.Accounting.Application.Accounts.Mappings;

namespace YourBrand.Accounting.Application.Accounts.Queries;

public record GetAccountQuery(int AccountNo) : IRequest<AccountDto>
{
    public class GetAccountQueryHandler : IRequestHandler<GetAccountQuery, AccountDto>
    {
        private readonly IAccountingContext context;

        public GetAccountQueryHandler(IAccountingContext context)
        {
            this.context = context;
        }

        public async Task<AccountDto> Handle(GetAccountQuery request, CancellationToken cancellationToken)
        {
            var account = await context.Accounts
                .Include(a => a.Entries)
                .AsNoTracking()
                .AsQueryable()
                .FirstAsync(a => a.AccountNo == request.AccountNo, cancellationToken);

            return MapAccount(account);
        }
    }
}
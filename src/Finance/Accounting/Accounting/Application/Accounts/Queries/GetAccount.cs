using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Accounting.Application.Common.Interfaces;

using static YourBrand.Accounting.Application.Accounts.Mappings;

namespace YourBrand.Accounting.Application.Accounts.Queries;

public record GetAccountQuery(string OrganizationId, int AccountNo) : IRequest<AccountDto>
{
    public class GetAccountQueryHandler(IAccountingContext context) : IRequestHandler<GetAccountQuery, AccountDto>
    {
        public async Task<AccountDto> Handle(GetAccountQuery request, CancellationToken cancellationToken)
        {
            var account = await context.Accounts
                .InOrganization(request.OrganizationId)
                .Include(a => a.Entries)
                .AsNoTracking()
                .AsQueryable()
                .FirstAsync(a => a.AccountNo == request.AccountNo, cancellationToken);

            return MapAccount(account);
        }
    }
}
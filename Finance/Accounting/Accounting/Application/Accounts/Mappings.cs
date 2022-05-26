using System.ComponentModel.DataAnnotations;

namespace Accounting.Application.Accounts;

public static class Mappings
{
    public static AccountDto MapAccount(Domain.Entities.Account account)
    {
        return new AccountDto
        {
            AccountNo = account.AccountNo,
            Class = new AccountClassDto
            {
                Id = (int)account.Class,
                Description = account.Class.GetAttribute<DisplayAttribute>()!.Name!
            },
            Name = account.Name,
            Description = account.Description,
            Balance =
                account.Entries.Sum(e => e.Debit.GetValueOrDefault())
                - account.Entries.Sum(e => e.Credit.GetValueOrDefault())
        };
    }
}
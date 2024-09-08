﻿using System.ComponentModel.DataAnnotations;

namespace YourBrand.Accounting.Application.Accounts;

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
            Balance = account.Debit - account.Credit
        };
    }
}
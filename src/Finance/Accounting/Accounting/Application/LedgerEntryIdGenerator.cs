using Microsoft.EntityFrameworkCore;

using YourBrand.Accounting.Application.Common.Interfaces;
using YourBrand.Accounting.Domain.Entities;
using YourBrand.Domain;

namespace YourBrand.Accounting.Application;

public class LedgerEntryIdGenerator(IAccountingContext accountingContext) : ILedgerEntryIdGenerator
{
    int id = 0;
    private bool isSet;

    public async Task<int> GetIdAsync(OrganizationId organizationId, CancellationToken cancellationToken = default)
    {
        if(isSet)
        {
            return ++id;
        }

        try
        {
            id = (await accountingContext.JournalEntries.InOrganization(organizationId).MaxAsync(x => x.Id, cancellationToken)) + 1;
        }
        catch (Exception) 
        {
            id = 1;
        }

        isSet = true;

        return id;
    }
}
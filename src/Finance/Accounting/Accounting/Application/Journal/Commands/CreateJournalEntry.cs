﻿using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Accounting.Application.Common.Interfaces;
using YourBrand.Accounting.Domain.Entities;

namespace YourBrand.Accounting.Application.Journal.Commands;

public record CreateJournalEntryCommand(string OrganizationId, string Description, int? InvoiceNo, List<CreateEntry> Entries) : IRequest<int>
{
    public class CreateJournalEntryCommandHandler(IAccountingContext context) : IRequestHandler<CreateJournalEntryCommand, int>
    {
        public async Task<int> Handle(CreateJournalEntryCommand request, CancellationToken cancellationToken)
        {
            if (request.Entries.Sum(x => x.Debit ?? -(x.Credit.GetValueOrDefault())) != 0)
            {
                throw new Exception("The sum of all entries must be 0.");
            }

            var journalEntry = new Domain.Entities.JournalEntry
            (
                DateTime.Now,
                request.Description,
                invoiceNo: request.InvoiceNo
            );
            journalEntry.OrganizationId = request.OrganizationId;

            foreach (var entryDto in request.Entries)
            {
                if (entryDto.Debit is null && entryDto.Credit is null
                    && entryDto.Debit is not null && entryDto.Credit is not null)
                {
                    throw new Exception("Cannot set both Debit and Credit.");
                }

                var account = await context.Accounts
                    .FirstAsync(a => a.AccountNo == entryDto.AccountNo, cancellationToken);

                LedgerEntry entry = null!;

                if (entryDto.Credit is not null)
                {
                    entry = journalEntry.AddCreditEntry(account, entryDto.Credit.GetValueOrDefault(), entryDto.Description);
                }
                else
                {
                    entry = journalEntry.AddDebitEntry(account, entryDto.Debit.GetValueOrDefault(), entryDto.Description);
                }

                //entry.AddDomainEvent(new EntryCreatedEvent(entry.Id));
            }

            context.JournalEntries.Add(journalEntry);

            await context.SaveChangesAsync(cancellationToken);

            return journalEntry.Id;
        }
    }
}
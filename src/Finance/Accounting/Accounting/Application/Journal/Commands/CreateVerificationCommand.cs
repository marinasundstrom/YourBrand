using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Accounting.Application.Common.Interfaces;
using YourBrand.Accounting.Domain.Entities;

namespace YourBrand.Accounting.Application.Journal.Commands;

public record CreateVerificationCommand(string OrganizationId, string Description, int? InvoiceNo, List<CreateEntry> Entries) : IRequest<int>
{
    public class CreateVerificationCommandHandler(IAccountingContext context, ILedgerEntryIdGenerator ledgerEntryIdGenerator) : IRequestHandler<CreateVerificationCommand, int>
    {
        public async Task<int> Handle(CreateVerificationCommand request, CancellationToken cancellationToken)
        {
            if (request.Entries.Sum(x => x.Debit ?? -(x.Credit.GetValueOrDefault())) != 0)
            {
                throw new Exception("The sum of all entries must be 0.");
            }

            int id = 0;

            try
            {
                id = (await context.Verifications.InOrganization(request.OrganizationId).MaxAsync(x => x.Id, cancellationToken)) + 1;
            }
            catch (Exception)
            {
                id = 1;
            }

            var verification = new Domain.Entities.Verification
            (
                id,
                DateTimeOffset.UtcNow,
                request.Description,
                invoiceNo: request.InvoiceNo
            );
            verification.OrganizationId = request.OrganizationId;

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
                    entry = await verification.AddCreditEntry(account, entryDto.Credit.GetValueOrDefault(), entryDto.Description, ledgerEntryIdGenerator);
                }
                else
                {
                    entry = await verification.AddDebitEntry(account, entryDto.Debit.GetValueOrDefault(), entryDto.Description, ledgerEntryIdGenerator);
                }

                //entry.AddDomainEvent(new EntryCreatedEvent(entry.Id));
            }

            context.Verifications.Add(verification);

            await context.SaveChangesAsync(cancellationToken);

            return verification.Id;
        }
    }
}
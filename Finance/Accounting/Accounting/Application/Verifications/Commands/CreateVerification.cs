using YourBrand.Accounting.Application.Common.Interfaces;
using YourBrand.Accounting.Domain.Events;

using MediatR;
using YourBrand.Accounting.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace YourBrand.Accounting.Application.Verifications.Commands;

public record CreateVerificationCommand(string Description, int? InvoiceId, List<CreateEntry> Entries) : IRequest<int>
{
    public class CreateVerificationCommandHandler : IRequestHandler<CreateVerificationCommand, int>
    {
        private readonly IAccountingContext context;

        public CreateVerificationCommandHandler(IAccountingContext context)
        {
            this.context = context;
        }

        public async Task<int> Handle(CreateVerificationCommand request, CancellationToken cancellationToken)
        {
            if (request.Entries.Sum(x => x.Debit ?? -(x.Credit.GetValueOrDefault())) != 0)
            {
                throw new Exception("The sum of all entries must be 0.");
            }

            var verification = new Domain.Entities.Verification
            (
                DateTime.Now,
                request.Description,
                invoiceId: request.InvoiceId
            );

            foreach (var entryDto in request.Entries)
            {
                if (entryDto.Debit is null && entryDto.Credit is null
                    && entryDto.Debit is not null && entryDto.Credit is not null)
                {
                    throw new Exception("Cannot set both Debit and Credit.");
                }

                var account = await context.Accounts
                    .FirstAsync(a => a.AccountNo == entryDto.AccountNo, cancellationToken);

                Entry entry = null!;

                if(entryDto.Credit is not null)
                {
                    entry = verification.AddCreditEntry(account, entryDto.Credit.GetValueOrDefault(), entryDto.Description);
                }
                else
                {
                    entry = verification.AddDebitEntry(account, entryDto.Debit.GetValueOrDefault(), entryDto.Description);
                }

                //entry.AddDomainEvent(new EntryCreatedEvent(entry.Id));
            }

            context.Verifications.Add(verification);

            await context.SaveChangesAsync(cancellationToken);

            return verification.Id;
        }
    }
}
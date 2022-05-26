using Accounting.Application.Common.Interfaces;
using Accounting.Domain.Events;

using MediatR;

namespace Accounting.Application.Verifications.Commands;

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
            {
                Description = request.Description,
                Date = DateTime.Now,
                InvoiceId = request.InvoiceId,
            };

            foreach (var entryDto in request.Entries)
            {
                if (entryDto.Debit is null && entryDto.Credit is null
                    && entryDto.Debit is not null && entryDto.Credit is not null)
                {
                    throw new Exception("Cannot set both Debit and Credit.");
                }

                var entry = new Domain.Entities.Entry
                {
                    Date = verification.Date,
                    AccountNo = entryDto.AccountNo,
                    Description = entryDto.Description ?? String.Empty,
                    Debit = entryDto.Debit,
                    Credit = entryDto.Credit
                };

                verification.Entries.Add(entry);

                entry.DomainEvents.Add(new EntryCreatedEvent(entry));
            }

            context.Verifications.Add(verification);

            await context.SaveChangesAsync(cancellationToken);

            return verification.Id;
        }
    }
}
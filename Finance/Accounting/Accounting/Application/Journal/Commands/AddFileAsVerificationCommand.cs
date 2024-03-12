using YourBrand.Accounting.Application.Common.Interfaces;
using YourBrand.Accounting.Domain.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;

using static YourBrand.Accounting.Application.Shared;

namespace YourBrand.Accounting.Application.Journal.Commands;

public record AddFileAsVerificationCommand(int JournalEntryId, string Name, string ContentType, string? Description, int? invoiceId, Stream Stream) : IRequest<string>
{
    public class AddFileAsVerificationCommandHandler : IRequestHandler<AddFileAsVerificationCommand, string>
    {
        private readonly IAccountingContext context;
        private readonly IBlobService blobService;

        public AddFileAsVerificationCommandHandler(IAccountingContext context, IBlobService blobService)
        {
            this.context = context;
            this.blobService = blobService;
        }

        public async Task<string> Handle(AddFileAsVerificationCommand request, CancellationToken cancellationToken)
        {
            var journalEntry = await context.JournalEntries
                .Include(v => v.Verifications)
                .FirstAsync(x => x.Id == request.JournalEntryId, cancellationToken);

            if (journalEntry.Verifications.Any())
            {
                throw new Exception("There is already an attachment to this verification.");
            }

            var blobName = $"{request.JournalEntryId}-{request.Name}";

            await blobService.UploadBloadAsync(blobName, request.Stream);

            var verification = new Verification();

            verification.Id = blobName;
            verification.Name = request.Name;
            verification.ContentType = request.ContentType;
            verification.Description = request.Description;
            verification.InvoiceNo = request.invoiceId;

            journalEntry.AddVerification(verification);

            await context.SaveChangesAsync(cancellationToken);

            return GetAttachmentUrl(verification.Id)!;
        }
    }
}
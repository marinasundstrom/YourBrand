using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Accounting.Application.Common.Interfaces;
using YourBrand.Accounting.Domain.Entities;

using static YourBrand.Accounting.Application.Shared;

namespace YourBrand.Accounting.Application.Journal.Commands;

public record AddFileAsVerificationCommand(string OrganizationId, int JournalEntryId, string Name, string ContentType, string? Description, int? invoiceId, Stream Stream) : IRequest<string>
{
    public class AddFileAsVerificationCommandHandler(IAccountingContext context, IBlobService blobService) : IRequestHandler<AddFileAsVerificationCommand, string>
    {
        public async Task<string> Handle(AddFileAsVerificationCommand request, CancellationToken cancellationToken)
        {
            var journalEntry = await context.JournalEntries
                .InOrganization(request.OrganizationId)
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
            verification.OrganizationId = request.OrganizationId;
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
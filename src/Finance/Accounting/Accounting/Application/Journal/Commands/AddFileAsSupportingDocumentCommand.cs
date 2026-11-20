using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Accounting.Application.Common.Interfaces;
using YourBrand.Accounting.Domain.Entities;

using static YourBrand.Accounting.Application.Shared;

namespace YourBrand.Accounting.Application.Journal.Commands;

public record AddFileAsSupportingDocumentCommand(string OrganizationId, int VerificationId, string Name, string ContentType, string? Description, int? invoiceId, Stream Stream) : IRequest<string>
{
    public class AddFileAsSupportingDocumentCommandHandler(IAccountingContext context, IBlobService blobService) : IRequestHandler<AddFileAsSupportingDocumentCommand, string>
    {
        public async Task<string> Handle(AddFileAsSupportingDocumentCommand request, CancellationToken cancellationToken)
        {
            var verification = await context.Verifications
                .InOrganization(request.OrganizationId)
                .Include(v => v.SupportingDocuments)
                .FirstAsync(x => x.Id == request.VerificationId, cancellationToken);

            if (verification.SupportingDocuments.Any())
            {
                throw new Exception("There is already an attachment to this supportingDocument.");
            }

            var blobName = $"{request.VerificationId}-{request.Name}";

            await blobService.UploadBloadAsync(blobName, request.Stream);

            var supportingDocument = new SupportingDocument(blobName);

            supportingDocument.OrganizationId = request.OrganizationId;
            supportingDocument.Name = request.Name;
            supportingDocument.ContentType = request.ContentType;
            supportingDocument.Description = request.Description;
            supportingDocument.InvoiceNo = request.invoiceId;

            verification.AddSupportingDocument(supportingDocument);

            await context.SaveChangesAsync(cancellationToken);

            return GetAttachmentUrl(supportingDocument.Id)!;
        }
    }
}
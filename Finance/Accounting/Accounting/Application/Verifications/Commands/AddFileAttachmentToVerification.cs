using Accounting.Application.Common.Interfaces;
using Accounting.Domain.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;

using static Accounting.Application.Shared;

namespace Accounting.Application.Verifications.Commands;

public record AddFileAttachmentToVerificationCommand(int VerificationId, string Name, string ContentType, string? Description, int? invoiceId, Stream Stream) : IRequest<string>
{
    public class AddFileAttachmentToVerificationCommandHandler : IRequestHandler<AddFileAttachmentToVerificationCommand, string>
    {
        private readonly IAccountingContext context;
        private readonly IBlobService blobService;

        public AddFileAttachmentToVerificationCommandHandler(IAccountingContext context, IBlobService blobService)
        {
            this.context = context;
            this.blobService = blobService;
        }

        public async Task<string> Handle(AddFileAttachmentToVerificationCommand request, CancellationToken cancellationToken)
        {
            var verification = await context.Verifications
                .Include(v => v.Attachments)
                .FirstAsync(x => x.Id == request.VerificationId, cancellationToken);

            if (verification.Attachments.Any())
            {
                throw new Exception("There is already an attachment to this verification.");
            }

            var blobName = $"{request.VerificationId}-{request.Name}";

            await blobService.UploadBloadAsync(blobName, request.Stream);

            var attachment = new Attachment();

            attachment.Id = blobName;
            attachment.Name = request.Name;
            attachment.ContentType = request.ContentType;
            attachment.Description = request.Description;
            attachment.InvoiceId = request.invoiceId;

            verification.Attachments.Add(attachment);

            await context.SaveChangesAsync(cancellationToken);

            return GetAttachmentUrl(attachment.Id)!;
        }
    }
}
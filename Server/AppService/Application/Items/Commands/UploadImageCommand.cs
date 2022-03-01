
using Skynet.Application.Common.Interfaces;
using Skynet.Domain.Events;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Skynet.Application.Items.Commands;

public record UploadImageCommand(string Id, Stream Stream) : IRequest<UploadImageResult>
{
    public class UploadImageCommandHandler : IRequestHandler<UploadImageCommand, UploadImageResult>
    {
        private readonly ICatalogContext context;
        private readonly IFileUploaderService _fileUploaderService;
        private readonly IItemsClient client;

        public UploadImageCommandHandler(ICatalogContext context, IFileUploaderService fileUploaderService, IItemsClient client)
        {
            this.context = context;
            this._fileUploaderService = fileUploaderService;
            this.client = client;
        }

        public async Task<UploadImageResult> Handle(UploadImageCommand request, CancellationToken cancellationToken)
        {
            var item = await context.Items.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (item is null)
            {
                return UploadImageResult.Successful;
            }

            string imageId = $"image-{request.Id}";

            await _fileUploaderService.UploadFileAsync(imageId, request.Stream, cancellationToken);

            item.DomainEvents.Add(new ItemImageUploadedEvent(item.Id));

            item.Image = imageId;
            await context.SaveChangesAsync(cancellationToken);

            return UploadImageResult.Successful;
        }
    }
}
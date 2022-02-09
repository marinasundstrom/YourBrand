
using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Events;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Items.Commands;

public class UploadImageCommand : IRequest<UploadImageResult>
{
    public string Id { get; set; }

    public Stream Stream { get; set; }

    public UploadImageCommand(string id, Stream stream)
    {
        Id = id;
        Stream = stream;
    }

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
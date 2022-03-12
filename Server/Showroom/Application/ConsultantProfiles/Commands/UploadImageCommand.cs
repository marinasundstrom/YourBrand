
using Skynet.Showroom.Application.Common.Interfaces;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Skynet.Showroom.Application.ConsultantProfiles.Commands;

public record UploadImageCommand(string Id, Stream Stream) : IRequest
{
    public class UploadImageCommandHandler : IRequestHandler<UploadImageCommand>
    {
        private readonly IShowroomContext context;
        private readonly IFileUploaderService _fileUploaderService;

        public UploadImageCommandHandler(IShowroomContext context, IFileUploaderService fileUploaderService)
        {
            this.context = context;
            this._fileUploaderService = fileUploaderService;
        }

        public async Task<Unit> Handle(UploadImageCommand request, CancellationToken cancellationToken)
        {
            var item = await context.ConsultantProfiles.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (item is null)
            {
                throw new Exception();
            }

            string imageId = $"image-{request.Id}";

            await _fileUploaderService.UploadFileAsync(imageId, request.Stream, cancellationToken);

            item.ProfileImage = imageId;
            await context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
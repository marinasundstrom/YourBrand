
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.Application.PersonProfiles.Commands;

public record UploadImageCommand(string Id, Stream Stream) : IRequest
{
    public class UploadImageCommandHandler(IShowroomContext context, IFileUploaderService fileUploaderService) : IRequestHandler<UploadImageCommand>
    {
        public async Task Handle(UploadImageCommand request, CancellationToken cancellationToken)
        {
            var item = await context.PersonProfiles.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (item is null)
            {
                throw new Exception();
            }

            string imageId = $"image-{request.Id}";

            await fileUploaderService.UploadFileAsync(imageId, request.Stream, cancellationToken);

            item.ProfileImage = imageId;
            await context.SaveChangesAsync(cancellationToken);

        }
    }
}
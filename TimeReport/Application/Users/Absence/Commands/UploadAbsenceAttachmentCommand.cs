
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Projects;

using static YourBrand.TimeReport.Application.Users.Absence.AbsenceHelpers;

namespace YourBrand.TimeReport.Application.Users.Absence.Commands;

public class UploadAbsenceAttachmentCommand : IRequest<string?>
{
    public UploadAbsenceAttachmentCommand(string absenceId, string name, Stream stream)
    {
        AbsenceId = absenceId;
        Name = name;
        Stream = stream;
    }

    public string AbsenceId { get; }

    public string Name { get; }

    public Stream Stream { get; }

    public class UploadAbsenceAttachmentCommandHandler : IRequestHandler<UploadAbsenceAttachmentCommand, string?>
    {
        private readonly ITimeReportContext _context;
        private readonly IBlobService _blobService;

        public UploadAbsenceAttachmentCommandHandler(ITimeReportContext context, IBlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        public async Task<string?> Handle(UploadAbsenceAttachmentCommand request, CancellationToken cancellationToken)
        {
            var absence = await _context.Absence
                .Include(x => x.Project)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.AbsenceId, cancellationToken);

            if (absence is null)
            {
                throw new Exception();
            }

            /*

            if (!string.IsNullOrEmpty(absence.Attachment))
            {
                throw new Exception();
            }

            */

            var blobName = $"{absence.Id}-{request.Name}";

            await _blobService.UploadBloadAsync(blobName, request.Stream);

            //absence.Attachment = blobName;

            await _context.SaveChangesAsync(cancellationToken);

            return null; //GetAttachmentUrl(absence.Attachment);
        }
    }
}
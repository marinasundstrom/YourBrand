using System;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Activities;
using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Projects;
using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Domain.Exceptions;

using static YourBrand.TimeReport.Application.TimeSheets.Constants;

namespace YourBrand.TimeReport.Application.TimeSheets.Commands;

public record UpdateEntryDetailsCommand(string TimeSheetId, string EntryId, string? Description) : IRequest<EntryDto>
{
    public class UpdateEntryDetailsCommandHandler : IRequestHandler<UpdateEntryDetailsCommand, EntryDto>
    {
        private readonly ITimeReportContext _context;

        public UpdateEntryDetailsCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<EntryDto> Handle(UpdateEntryDetailsCommand request, CancellationToken cancellationToken)
        {
            var timeSheet = await _context.TimeSheets.GetTimeSheetAsync(request.TimeSheetId, cancellationToken);

            if (timeSheet is null)
            {
                throw new TimeSheetNotFoundException(request.TimeSheetId);
            }

            if (timeSheet.Status != TimeSheetStatus.Open)
            {
                throw new TimeSheetClosedException(request.TimeSheetId);
            }

            var entry = timeSheet.Entries.FirstOrDefault(e => e.Id == request.EntryId);

            if (entry is null)
            {
                throw new EntryNotFoundException(request.EntryId);
            }

            if (entry.MonthGroup.Status == EntryStatus.Locked)
            {
                throw new MonthLockedException(request.TimeSheetId);
            }

            entry.Description = request.Description;

            await _context.SaveChangesAsync(cancellationToken);

            return entry.ToDto();
        }
    }
}
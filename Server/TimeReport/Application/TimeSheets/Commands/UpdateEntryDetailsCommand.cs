using System;

using MediatR;

using Microsoft.EntityFrameworkCore;

using TimeReport.Application.Activities;
using TimeReport.Application.Common.Interfaces;
using TimeReport.Application.Projects;
using TimeReport.Domain.Entities;
using TimeReport.Domain.Exceptions;

using static TimeReport.Application.TimeSheets.Constants;

namespace TimeReport.Application.TimeSheets.Commands;

public class UpdateEntryDetailsCommand : IRequest<EntryDto>
{
    public UpdateEntryDetailsCommand(string timeSheetId, string entryId, string? description)
    {
        TimeSheetId = timeSheetId;
        EntryId = entryId;
        Description = description;
    }

    public string TimeSheetId { get; }

    public string EntryId { get; }

    public string? Description { get; }

    public class UpdateEntryDetailsCommandHandler : IRequestHandler<UpdateEntryDetailsCommand, EntryDto>
    {
        private readonly ITimeReportContext _context;

        public UpdateEntryDetailsCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<EntryDto> Handle(UpdateEntryDetailsCommand request, CancellationToken cancellationToken)
        {
            var timeSheet = await _context.TimeSheets
             .Include(x => x.Entries)
             .ThenInclude(x => x.MonthGroup)
             .Include(x => x.Entries)
             .ThenInclude(x => x.Project)
             .Include(x => x.Entries)
             .ThenInclude(x => x.Activity)
             .Include(x => x.Entries)
             .ThenInclude(x => x.Activity)
             .ThenInclude(x => x.Project)
             .AsSplitQuery()
             .FirstAsync(x => x.Id == request.TimeSheetId, cancellationToken);

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

            var e = entry;

            return new EntryDto(e.Id, new ProjectDto(e.Project.Id, e.Project.Name, e.Project.Description), new ActivityDto(e.Activity.Id, e.Activity.Name, e.Activity.Description, e.Activity.HourlyRate, new ProjectDto(e.Activity.Project.Id, e.Activity.Project.Name, e.Activity.Project.Description)), e.Date.ToDateTime(TimeOnly.Parse("01:00")), e.Hours, e.Description, (EntryStatusDto)e.MonthGroup.Status);
        }
    }
}
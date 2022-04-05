
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Domain.Exceptions;

namespace YourBrand.TimeReport.Application.TimeSheets.Commands;

public class DeleteActivityCommand : IRequest
{
    public DeleteActivityCommand(string timeSheetId, string activityId)
    {
        TimeSheetId = timeSheetId;
        ActivityId = activityId;
    }

    public string TimeSheetId { get; }

    public string ActivityId { get; }

    public class DeleteActivityCommandHandler : IRequestHandler<DeleteActivityCommand>
    {
        private readonly ITimeReportContext _context;

        public DeleteActivityCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteActivityCommand request, CancellationToken cancellationToken)
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

            var activity = await _context!.Activities.FirstOrDefaultAsync(x => x.Id == request.ActivityId, cancellationToken);

            if (activity is null)
            {
                throw new ActivityNotFoundException(request.ActivityId);
            }

            var entries = timeSheet.Entries.Where(e => e.Activity.Id == request.ActivityId);

            foreach (var entry in entries.Where(e => e.Status == EntryStatus.Unlocked))
            {
                _context.Entries.Remove(entry);
            }

            if (entries.All(e => e.Status == EntryStatus.Unlocked))
            {
                var timeSheetActivity = await _context.TimeSheetActivities
                    .FirstOrDefaultAsync(x => x.TimeSheet.Id == timeSheet.Id && x.Activity.Id == activity.Id, cancellationToken);

                if (timeSheetActivity is not null)
                {
                    _context.TimeSheetActivities.Remove(timeSheetActivity);
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
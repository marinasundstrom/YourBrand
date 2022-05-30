
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Domain.Exceptions;

namespace YourBrand.TimeReport.Application.TimeSheets.Commands;

public record DeleteActivityCommand(string TimeSheetId, string ActivityId) : IRequest
{
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

            var entries = timeSheet.GetEntriesByActivityId(activity.Id);

            if (entries.All(e => e.Status == EntryStatus.Unlocked))
            {
                var timeSheetActivity = timeSheet.GetActivity(activity.Id);

                if (timeSheetActivity is not null)
                {
                    timeSheet.DeleteActivity(timeSheetActivity);
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
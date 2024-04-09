
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain;
using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Domain.Exceptions;
using YourBrand.TimeReport.Domain.Repositories;

namespace YourBrand.TimeReport.Application.TimeSheets.Commands;

public record DeleteActivityCommand(string TimeSheetId, string ActivityId) : IRequest
{
    public class DeleteActivityCommandHandler(ITimeSheetRepository timeSheetRepository, IUnitOfWork unitOfWork, ITimeReportContext context) : IRequestHandler<DeleteActivityCommand>
    {
        public async Task Handle(DeleteActivityCommand request, CancellationToken cancellationToken)
        {
            var timeSheet = await timeSheetRepository.GetTimeSheet(request.TimeSheetId, cancellationToken);

            if (timeSheet is null)
            {
                throw new TimeSheetNotFoundException(request.TimeSheetId);
            }

            if (timeSheet.Status != TimeSheetStatus.Open)
            {
                throw new TimeSheetClosedException(request.TimeSheetId);
            }

            var activity = await context!.Activities.FirstOrDefaultAsync(x => x.Id == request.ActivityId, cancellationToken);

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

            await unitOfWork.SaveChangesAsync(cancellationToken);

        }
    }
}
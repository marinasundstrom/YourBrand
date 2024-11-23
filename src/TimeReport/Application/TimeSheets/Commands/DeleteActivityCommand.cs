
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain;
using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Domain.Repositories;

namespace YourBrand.TimeReport.Application.TimeSheets.Commands;

public record DeleteActivityCommand(string OrganizationId, string TimeSheetId, string ActivityId) : IRequest<Result>
{
    public class DeleteActivityCommandHandler(ITimeSheetRepository timeSheetRepository, IUnitOfWork unitOfWork, ITimeReportContext context) : IRequestHandler<DeleteActivityCommand, Result>
    {
        public async Task<Result> Handle(DeleteActivityCommand request, CancellationToken cancellationToken)
        {
            var timeSheet = await timeSheetRepository.GetTimeSheet(request.TimeSheetId, cancellationToken);

            if (timeSheet is null)
            {
                return new TimeSheetNotFound(request.TimeSheetId);
            }

            if (timeSheet.Status != TimeSheetStatus.Open)
            {
                return new TimeSheetClosed(request.TimeSheetId);
            }

            var activity = await context!.Activities.FirstOrDefaultAsync(x => x.Id == request.ActivityId, cancellationToken);

            if (activity is null)
            {
                return new ActivityNotFound(request.ActivityId);
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

            return Result.Success;
        }
    }
}
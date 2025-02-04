
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain;
using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Domain.Repositories;

namespace YourBrand.TimeReport.Application.TimeSheets.Commands;

public record DeleteTaskCommand(string OrganizationId, string TimeSheetId, string TaskId) : IRequest<Result>
{
    public class DeleteTaskCommandHandler(ITimeSheetRepository timeSheetRepository, IUnitOfWork unitOfWork, ITimeReportContext context) : IRequestHandler<DeleteTaskCommand, Result>
    {
        public async Task<Result> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
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

            var task = await context!.Tasks.FirstOrDefaultAsync(x => x.Id == request.TaskId, cancellationToken);

            if (task is null)
            {
                return new TaskNotFound(request.TaskId);
            }

            var entries = timeSheet.GetEntriesByTaskId(task.Id);

            if (entries.All(e => e.Status == EntryStatus.Unlocked))
            {
                var timeSheetTask = timeSheet.GetTask(task.Id);

                if (timeSheetTask is not null)
                {
                    timeSheet.DeleteTask(timeSheetTask);
                }
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}
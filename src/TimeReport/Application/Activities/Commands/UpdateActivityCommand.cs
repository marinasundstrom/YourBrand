
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Activities.Commands;

public record UpdateActivityCommand(string ActivityId, string Name, string ActivityTypeId, string? Description, decimal? HourlyRate) : IRequest<ActivityDto>
{
    public class UpdateActivityCommandHandler(ITimeReportContext context) : IRequestHandler<UpdateActivityCommand, ActivityDto>
    {
        public async Task<ActivityDto> Handle(UpdateActivityCommand request, CancellationToken cancellationToken)
        {
            var activity = await context.Activities
                .Include(x => x.Project)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.ActivityId, cancellationToken);

            if (activity is null)
            {
                throw new Exception();
            }

            activity.Name = request.Name;
            activity.ActivityType = await context.ActivityTypes.FirstAsync(at => at.Id == request.ActivityTypeId);
            activity.Description = request.Description;
            activity.HourlyRate = request.HourlyRate;

            await context.SaveChangesAsync(cancellationToken);

            activity = await context.Activities
               .Include(x => x.ActivityType)
               .Include(x => x.Project)
               .ThenInclude(x => x.Organization)
               .AsNoTracking()
               .AsSplitQuery()
               .FirstOrDefaultAsync(x => x.Id == request.ActivityId, cancellationToken);

            return activity.ToDto();
        }
    }
}

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Projects;

namespace YourBrand.TimeReport.Application.Activities.Commands;

public class UpdateActivityCommand : IRequest<ActivityDto>
{
    public UpdateActivityCommand(string activityId, string name, string activityTypeId, string? description, decimal? hourlyRate)
    {
        ActivityId = activityId;
        Name = name;
        ActivityTypeId = activityTypeId;
        Description = description;
        HourlyRate = hourlyRate;
    }

    public string ActivityId { get; }
    public string Name { get; }
    public string ActivityTypeId { get; }
    public string? Description { get; }
    public decimal? HourlyRate { get; }

    public class UpdateActivityCommandHandler : IRequestHandler<UpdateActivityCommand, ActivityDto>
    {
        private readonly ITimeReportContext _context;

        public UpdateActivityCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<ActivityDto> Handle(UpdateActivityCommand request, CancellationToken cancellationToken)
        {
            var activity = await _context.Activities
                .Include(x => x.Project)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.ActivityId, cancellationToken);

            if (activity is null)
            {
                throw new Exception();
            }

            activity.Name = request.Name;
            activity.ActivityType = await _context.ActivityTypes.FirstAsync(at => at.Id == request.ActivityTypeId);
            activity.Description = request.Description;
            activity.HourlyRate = request.HourlyRate;

            await _context.SaveChangesAsync(cancellationToken);

            return activity.ToDto();
        }
    }
}
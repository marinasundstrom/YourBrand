
using MediatR;

using Microsoft.EntityFrameworkCore;

using TimeReport.Application.Common.Interfaces;
using TimeReport.Application.Projects;

namespace TimeReport.Application.Activities.Commands;

public class UpdateActivityCommand : IRequest<ActivityDto>
{
    public UpdateActivityCommand(string activityId, string name, string? description, decimal? hourlyRate)
    {
        ActivityId = activityId;
        Name = name;
        Description = description;
        HourlyRate = hourlyRate;
    }

    public string ActivityId { get; }
    public string Name { get; }
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
            activity.Description = request.Description;
            activity.HourlyRate = request.HourlyRate;

            await _context.SaveChangesAsync(cancellationToken);

            return new ActivityDto(activity.Id, activity.Name, activity.Description, activity.HourlyRate, new ProjectDto(activity.Project.Id, activity.Project.Name, activity.Project.Description));
        }
    }
}
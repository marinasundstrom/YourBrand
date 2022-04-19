
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Projects;
using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Application.Activities.Commands;

public record CreateActivityCommand(string ProjectId, string Name, string ActivityTypeId, string? Description, decimal? HourlyRate) : IRequest<ActivityDto>
{
    public class CreateActivityCommandHandler : IRequestHandler<CreateActivityCommand, ActivityDto>
    {
        private readonly ITimeReportContext _context;

        public CreateActivityCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<ActivityDto> Handle(CreateActivityCommand request, CancellationToken cancellationToken)
        {
            var project = await _context.Projects
               .AsSplitQuery()
               .FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);

            if (project is null)
            {
                throw new Exception();
            }

            var activity = new Activity
            {
                Id = Guid.NewGuid().ToString(),
                Name = request.Name,
                ActivityType = await _context.ActivityTypes.FirstAsync(at => at.Id == request.ActivityTypeId),
                Description = request.Description,
                Project = project,
                HourlyRate = request.HourlyRate
            };

            _context.Activities.Add(activity);

            await _context.SaveChangesAsync(cancellationToken);

            return activity.ToDto();
        }
    }
}
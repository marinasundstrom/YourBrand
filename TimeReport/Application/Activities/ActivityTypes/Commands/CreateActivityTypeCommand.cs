
using MediatR;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Application.Activities.ActivityTypes.Commands;

public record CreateActivityTypeCommand(string Name, string? Description, bool ExcludeHours) : IRequest<ActivityTypeDto>
{
    public class CreateActivityCommandHandler : IRequestHandler<CreateActivityTypeCommand, ActivityTypeDto>
    {
        private readonly ITimeReportContext _context;

        public CreateActivityCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<ActivityTypeDto> Handle(CreateActivityTypeCommand request, CancellationToken cancellationToken)
        {
            /*
            var project = await _context.Projects
               .AsSplitQuery()
               .FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);

            if (project is null)
            {
                throw new Exception();
            }
            */

            var activityType = new ActivityType
            {
                Id = Guid.NewGuid().ToString(),
                Name = request.Name,
                Description = request.Description,
                //Project = project,
                ExcludeHours = request.ExcludeHours
            };

            _context.ActivityTypes.Add(activityType);

            await _context.SaveChangesAsync(cancellationToken);

            return activityType.ToDto();
        }
    }
}
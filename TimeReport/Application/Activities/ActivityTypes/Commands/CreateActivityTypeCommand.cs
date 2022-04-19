
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Projects;
using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Application.Activities.ActivityTypes.Commands;

public class CreateActivityTypeCommand : IRequest<ActivityTypeDto>
{
    public CreateActivityTypeCommand(string name, string? description, bool excludeHours)
    {
        Name = name;
        Description = description;
        ExcludeHours = excludeHours;
    }

    public string Name { get; }
    public string? Description { get; }
    public bool ExcludeHours { get; }

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
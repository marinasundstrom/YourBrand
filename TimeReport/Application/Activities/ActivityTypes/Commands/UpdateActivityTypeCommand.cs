
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Projects;

namespace YourBrand.TimeReport.Application.Activities.ActivityTypes.Commands;

public record UpdateActivityTypeCommand(string ActivityId, string Name, string? Description, bool ExcludeHours) : IRequest<ActivityTypeDto>
{
    public class UpdateActivityCommandHandler : IRequestHandler<UpdateActivityTypeCommand, ActivityTypeDto>
    {
        private readonly ITimeReportContext _context;

        public UpdateActivityCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<ActivityTypeDto> Handle(UpdateActivityTypeCommand request, CancellationToken cancellationToken)
        {
            var activityType = await _context.ActivityTypes
                .Include(x => x.Project)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.ActivityId, cancellationToken);

            if (activityType is null)
            {
                throw new Exception();
            }

            activityType.Name = request.Name;
            activityType.Description = request.Description;
            activityType.ExcludeHours = request.ExcludeHours;

            await _context.SaveChangesAsync(cancellationToken);

            return activityType.ToDto();
        }
    }
}
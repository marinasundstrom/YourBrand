
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Projects;
using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Application.Activities.ActivityTypes.Commands;

public record UpdateActivityTypeCommand(string ActivityId, string Name, string? Description, string OrganizationId, string? ProjectId, bool ExcludeHours) : IRequest<ActivityTypeDto>
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

            Project? project = null;

            Organization organization = await _context.Organizations
                    .AsSplitQuery()
                    .FirstAsync(x => x.Id == request.OrganizationId, cancellationToken);
               
            if (request.ProjectId is not null)
            {
                project = await _context.Projects
                        .AsSplitQuery()
                        .FirstAsync(x => x.Organization.Id == request.OrganizationId && x.Id == request.ProjectId, cancellationToken);
            }

            activityType.Name = request.Name;
            activityType.Description = request.Description;
            activityType.Organization = organization;
            activityType.Project = project;
            activityType.ExcludeHours = request.ExcludeHours;

            await _context.SaveChangesAsync(cancellationToken);

            return activityType.ToDto();
        }
    }
}
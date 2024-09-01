
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Application.Activities.ActivityTypes.Commands;

public record UpdateActivityTypeCommand(string OrganizationId, string ActivityId, string Name, string? Description, string? ProjectId, bool ExcludeHours) : IRequest<ActivityTypeDto>
{
    public class UpdateActivityCommandHandler(ITimeReportContext context) : IRequestHandler<UpdateActivityTypeCommand, ActivityTypeDto>
    {
        public async Task<ActivityTypeDto> Handle(UpdateActivityTypeCommand request, CancellationToken cancellationToken)
        {
            var activityType = await context.ActivityTypes
                .InOrganization(request.OrganizationId)
                .Include(x => x.Project)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.ActivityId, cancellationToken);

            if (activityType is null)
            {
                throw new Exception();
            }

            Project? project = null;

            Organization organization = await context.Organizations
                    .AsSplitQuery()
                    .FirstAsync(x => x.Id == request.OrganizationId, cancellationToken);

            if (request.ProjectId is not null)
            {
                project = await context.Projects
                        .AsSplitQuery()
                        .FirstAsync(x => x.Organization.Id == request.OrganizationId && x.Id == request.ProjectId, cancellationToken);
            }

            activityType.Name = request.Name;
            activityType.Description = request.Description;
            activityType.Organization = organization;
            activityType.Project = project;
            activityType.ExcludeHours = request.ExcludeHours;

            await context.SaveChangesAsync(cancellationToken);

            return activityType.ToDto();
        }
    }
}
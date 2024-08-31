
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Application.Activities.ActivityTypes.Commands;

public record CreateActivityTypeCommand(string OrganizationId, string Name, string? Description, string? ProjectId, bool ExcludeHours) : IRequest<ActivityTypeDto>
{
    public class CreateActivityCommandHandler(ITimeReportContext context) : IRequestHandler<CreateActivityTypeCommand, ActivityTypeDto>
    {
        public async Task<ActivityTypeDto> Handle(CreateActivityTypeCommand request, CancellationToken cancellationToken)
        {
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

            var activityType = new ActivityType(request.Name, request.Description)
            {
                Organization = organization,
                Project = project,
                ExcludeHours = request.ExcludeHours
            };

            context.ActivityTypes.Add(activityType);

            await context.SaveChangesAsync(cancellationToken);

            return activityType.ToDto();
        }
    }
}

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Application.Activities.Commands;

public record CreateActivityCommand(string OrganizationId, string ProjectId, string Name, string ActivityTypeId, string? Description, decimal? HourlyRate) : IRequest<ActivityDto>
{
    public class CreateActivityCommandHandler(ITimeReportContext context) : IRequestHandler<CreateActivityCommand, ActivityDto>
    {
        public async Task<ActivityDto> Handle(CreateActivityCommand request, CancellationToken cancellationToken)
        {
            var project = await context.Projects
               .InOrganization(request.OrganizationId)
               .AsSplitQuery()
               .Include(at => at.Organization)
               .ThenInclude(at => at.CreatedBy)
               .Include(at => at.Organization)
               .ThenInclude(at => at.LastModifiedBy)
               .Include(at => at.Organization)
               .ThenInclude(at => at.DeletedBy)
               .Include(at => at.CreatedBy)
               .Include(at => at.LastModifiedBy)
               .Include(at => at.DeletedBy)
               .FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);

            if (project is null)
            {
                throw new Exception();
            }

            var activityType = await context.ActivityTypes
                    .AsSingleQuery()
                    .IncludeAll()
                    .FirstAsync(at => at.Id == request.ActivityTypeId);

            var activity = new Activity(request.Name, activityType, request.Description)
            {
                OrganizationId = request.OrganizationId,
                Project = project,
                HourlyRate = request.HourlyRate
            };

            context.Activities.Add(activity);

            await context.SaveChangesAsync(cancellationToken);

            return activity.ToDto();
        }
    }
}
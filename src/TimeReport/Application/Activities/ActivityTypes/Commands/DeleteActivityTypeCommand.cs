
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Activities.ActivityTypes.Commands;

public record DeleteActivityTypeCommand(string OrganizationId, string ActivityId) : IRequest
{
    public class DeleteActivityCommandHandler(ITimeReportContext context) : IRequestHandler<DeleteActivityTypeCommand>
    {
        public async Task Handle(DeleteActivityTypeCommand request, CancellationToken cancellationToken)
        {
            var activityType = await context.ActivityTypes
                .InOrganization(request.OrganizationId)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.ActivityId, cancellationToken);

            if (activityType is null)
            {
                throw new Exception();
            }

            context.ActivityTypes.Remove(activityType);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}
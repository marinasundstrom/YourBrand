
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Activities.Commands;

public record DeleteActivityCommand(string ActivityId) : IRequest
{
    public class DeleteActivityCommandHandler(ITimeReportContext context) : IRequestHandler<DeleteActivityCommand>
    {
        public async Task Handle(DeleteActivityCommand request, CancellationToken cancellationToken)
        {
            var activity = await context.Activities
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.ActivityId, cancellationToken);

            if (activity is null)
            {
                throw new Exception();
            }

            context.Activities.Remove(activity);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}
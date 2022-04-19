
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Activities.ActivityTypes.Commands;

public class DeleteActivityTypeCommand : IRequest
{
    public DeleteActivityTypeCommand(string activityId)
    {
        ActivityId = activityId;
    }

    public string ActivityId { get; }

    public class DeleteActivityCommandHandler : IRequestHandler<DeleteActivityTypeCommand>
    {
        private readonly ITimeReportContext _context;

        public DeleteActivityCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteActivityTypeCommand request, CancellationToken cancellationToken)
        {
            var activityType = await _context.ActivityTypes
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.ActivityId, cancellationToken);

            if (activityType is null)
            {
                throw new Exception();
            }

            _context.ActivityTypes.Remove(activityType);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
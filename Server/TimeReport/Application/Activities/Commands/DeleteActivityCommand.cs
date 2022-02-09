
using MediatR;

using Microsoft.EntityFrameworkCore;

using TimeReport.Application.Common.Interfaces;

namespace TimeReport.Application.Activities.Commands;

public class DeleteActivityCommand : IRequest
{
    public DeleteActivityCommand(string activityId)
    {
        ActivityId = activityId;
    }

    public string ActivityId { get; }

    public class DeleteActivityCommandHandler : IRequestHandler<DeleteActivityCommand>
    {
        private readonly ITimeReportContext _context;

        public DeleteActivityCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteActivityCommand request, CancellationToken cancellationToken)
        {
            var activity = await _context.Activities
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.ActivityId, cancellationToken);

            if (activity is null)
            {
                throw new Exception();
            }

            _context.Activities.Remove(activity);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
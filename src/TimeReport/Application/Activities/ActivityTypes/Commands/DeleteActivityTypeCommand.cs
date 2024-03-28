
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Activities.ActivityTypes.Commands;

public record DeleteActivityTypeCommand(string ActivityId) : IRequest
{
    public class DeleteActivityCommandHandler : IRequestHandler<DeleteActivityTypeCommand>
    {
        private readonly ITimeReportContext _context;

        public DeleteActivityCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteActivityTypeCommand request, CancellationToken cancellationToken)
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

        }
    }
}
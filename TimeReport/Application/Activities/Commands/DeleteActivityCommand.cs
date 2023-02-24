
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Activities.Commands;

public record DeleteActivityCommand(string ActivityId) : IRequest
{
    public class DeleteActivityCommandHandler : IRequestHandler<DeleteActivityCommand>
    {
        private readonly ITimeReportContext _context;

        public DeleteActivityCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteActivityCommand request, CancellationToken cancellationToken)
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

        }
    }
}
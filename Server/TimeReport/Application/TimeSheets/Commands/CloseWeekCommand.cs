
using MediatR;

using Microsoft.EntityFrameworkCore;

using Skynet.TimeReport.Application.Common.Interfaces;
using Skynet.TimeReport.Domain.Entities;
using Skynet.TimeReport.Domain.Exceptions;

namespace Skynet.TimeReport.Application.TimeSheets.Commands;

public class CloseWeekCommand : IRequest
{
    public CloseWeekCommand(string timeSheetId)
    {
        TimeSheetId = timeSheetId;
    }

    public string TimeSheetId { get; }

    public class CloseWeekCommandHandler : IRequestHandler<CloseWeekCommand>
    {
        private readonly ITimeReportContext _context;

        public CloseWeekCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(CloseWeekCommand request, CancellationToken cancellationToken)
        {
            var timeSheet = await _context.TimeSheets
                .Include(x => x.Entries)
                .ThenInclude(x => x.Project)
                .Include(x => x.Entries)
                .ThenInclude(x => x.Activity)
                .Include(x => x.Entries)
                .ThenInclude(x => x.Activity)
                .ThenInclude(x => x.Project)
                .AsSplitQuery()
                .FirstAsync(x => x.Id == request.TimeSheetId, cancellationToken);

            if (timeSheet is null)
            {
                throw new TimeSheetNotFoundException(request.TimeSheetId);
            }

            timeSheet.Status = TimeSheetStatus.Closed;
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}


using MediatR;

using Microsoft.EntityFrameworkCore;

using TimeReport.Application.Common.Interfaces;
using TimeReport.Domain.Entities;
using TimeReport.Domain.Exceptions;

namespace TimeReport.Application.TimeSheets.Commands;

public class UpdateTimeSheetStatusCommand : IRequest
{
    public UpdateTimeSheetStatusCommand(string timeSheetId)
    {
        TimeSheetId = timeSheetId;
    }

    public string TimeSheetId { get; }

    public class UpdateTimeSheetStatusCommandHandler : IRequestHandler<UpdateTimeSheetStatusCommand>
    {
        private readonly ITimeReportContext _context;

        public UpdateTimeSheetStatusCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateTimeSheetStatusCommand request, CancellationToken cancellationToken)
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
                .FirstAsync(x => x.Id == request.TimeSheetId);

            if (timeSheet is null)
            {
                throw new TimeSheetNotFoundException(request.TimeSheetId);
            }

            timeSheet.Status = TimeSheetStatus.Closed;
            await _context.SaveChangesAsync();

            return Unit.Value;
        }
    }
}

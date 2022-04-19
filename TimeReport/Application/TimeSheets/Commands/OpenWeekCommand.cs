
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Domain.Exceptions;

namespace YourBrand.TimeReport.Application.TimeSheets.Commands;

public record OpenWeekCommand(string TimeSheetId) : IRequest
{
    public class OpenWeekCommandHandler : IRequestHandler<OpenWeekCommand>
    {
        private readonly ITimeReportContext _context;

        public OpenWeekCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(OpenWeekCommand request, CancellationToken cancellationToken)
        {
            var timeSheet = await _context.TimeSheets.GetTimeSheetAsync(request.TimeSheetId, cancellationToken);

            if (timeSheet is null)
            {
                throw new TimeSheetNotFoundException(request.TimeSheetId);
            }

            timeSheet.UpdateStatus(TimeSheetStatus.Open);

            foreach (var entry in timeSheet.Entries)
            {
                entry.UpdateStatus(EntryStatus.Unlocked);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}

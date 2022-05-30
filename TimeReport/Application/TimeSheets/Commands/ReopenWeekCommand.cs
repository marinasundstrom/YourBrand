
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Domain.Exceptions;

namespace YourBrand.TimeReport.Application.TimeSheets.Commands;

public record ReopenWeekCommand(string TimeSheetId) : IRequest
{
    public class ReopenWeekCommandHandler : IRequestHandler<ReopenWeekCommand>
    {
        private readonly ITimeReportContext _context;

        public ReopenWeekCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(ReopenWeekCommand request, CancellationToken cancellationToken)
        {
            var timeSheet = await _context.TimeSheets.GetTimeSheetAsync(request.TimeSheetId, cancellationToken);

            if (timeSheet is null)
            {
                throw new TimeSheetNotFoundException(request.TimeSheetId);
            }

            timeSheet.Reopen();

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}


using MediatR;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Domain.Exceptions;

namespace YourBrand.TimeReport.Application.TimeSheets.Commands;

public record UpdateTimeSheetStatusCommand(string TimeSheetId) : IRequest
{
    public class UpdateTimeSheetStatusCommandHandler : IRequestHandler<UpdateTimeSheetStatusCommand>
    {
        private readonly ITimeReportContext _context;

        public UpdateTimeSheetStatusCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateTimeSheetStatusCommand request, CancellationToken cancellationToken)
        {
            var timeSheet = await _context.TimeSheets.GetTimeSheetAsync(request.TimeSheetId, cancellationToken);

            if (timeSheet is null)
            {
                throw new TimeSheetNotFoundException(request.TimeSheetId);
            }

            timeSheet.UpdateStatus(TimeSheetStatus.Closed);
            await _context.SaveChangesAsync();

            return Unit.Value;
        }
    }
}

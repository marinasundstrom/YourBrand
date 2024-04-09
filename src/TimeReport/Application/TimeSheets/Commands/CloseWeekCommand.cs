
using MediatR;

using YourBrand.TimeReport.Domain;
using YourBrand.TimeReport.Domain.Exceptions;
using YourBrand.TimeReport.Domain.Repositories;

namespace YourBrand.TimeReport.Application.TimeSheets.Commands;

public record CloseWeekCommand(string TimeSheetId) : IRequest
{
    public class CloseWeekCommandHandler(ITimeSheetRepository timeSheetRepository, IUnitOfWork unitOfWork) : IRequestHandler<CloseWeekCommand>
    {
        public async Task Handle(CloseWeekCommand request, CancellationToken cancellationToken)
        {
            var timeSheet = await timeSheetRepository.GetTimeSheet(request.TimeSheetId, cancellationToken);

            if (timeSheet is null)
            {
                throw new TimeSheetNotFoundException(request.TimeSheetId);
            }

            timeSheet.Close();

            await unitOfWork.SaveChangesAsync(cancellationToken);

        }
    }
}
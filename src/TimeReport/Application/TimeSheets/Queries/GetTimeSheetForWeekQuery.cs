using System.Globalization;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain;
using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Domain.Repositories;

namespace YourBrand.TimeReport.Application.TimeSheets.Queries;

public record GetTimeSheetForWeekQuery(string OrganizationId, int Year, int Week, string? UserId) : IRequest<Result<TimeSheetDto?>>
{
    public sealed class GetTimeSheetForWeekQueryHandler(ITimeSheetRepository timeSheetRepository, IReportingPeriodRepository reportingPeriodRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, ITimeReportContext context, IUserContext userContext) : IRequestHandler<GetTimeSheetForWeekQuery, Result<TimeSheetDto?>>
    {
        public async Task<Result<TimeSheetDto?>> Handle(GetTimeSheetForWeekQuery request, CancellationToken cancellationToken)
        {
            var query = timeSheetRepository.GetTimeSheets()
                .AsSplitQuery();

            string? userId = request.UserId ?? userContext.UserId;

            query = query.Where(x => x.UserId == userId);

            var timeSheet = await query.FirstOrDefaultAsync(x => x.Year == request.Year && x.Week == request.Week, cancellationToken);

            if (timeSheet is null)
            {
                User? user = await userRepository.GetUser(userId!, cancellationToken);

                userId = user?.Id;

                var startDate = ISOWeek.ToDateTime(request.Year, request.Week, DayOfWeek.Monday);

                timeSheet = new TimeSheet(user!, request.Year, request.Week);
                timeSheet.OrganizationId = request.OrganizationId;

                context.TimeSheets.Add(timeSheet);

                await unitOfWork.SaveChangesAsync(cancellationToken);
            }

            var periods = await reportingPeriodRepository.GetReportingPeriodForTimeSheet(timeSheet, cancellationToken);

            return timeSheet.ToDto(periods);
        }
    }
}
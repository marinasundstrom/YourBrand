using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Meetings.Domain;
using YourBrand.Meetings.Features;
using YourBrand.Meetings.Models;

namespace YourBrand.Meetings.Features.Queries;

public record GetMeetingById(string OrganizationId, int Id) : IRequest<Result<MeetingDto>>
{
    public class Handler(IApplicationDbContext context) : IRequestHandler<GetMeetingById, Result<MeetingDto>>
    {
        public async Task<Result<MeetingDto>> Handle(GetMeetingById request, CancellationToken cancellationToken)
        {
            var meeting = await context.Meetings
                .InOrganization(request.OrganizationId)
                .AsNoTracking()
                .AsSplitQuery()
                .Include(x => x.Attendees.OrderBy(x => x.Order))
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (meeting is null)
            {
                return Errors.Meetings.MeetingNotFound;
            }

            return meeting.ToDto();
        }
    }
}
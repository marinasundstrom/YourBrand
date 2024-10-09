using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Meetings.Features;
using YourBrand.Meetings.Models;
using YourBrand.Meetings.Domain;
 
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
                .Include(x => x.Participants.OrderBy(x => x.Created))
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if(meeting is null) 
            {
                return Errors.Meetings.MeetingNotFound;
            }

            return meeting.ToDto();
        }
    }
}
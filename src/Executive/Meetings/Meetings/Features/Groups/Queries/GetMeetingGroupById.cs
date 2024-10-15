using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Meetings.Features;
using YourBrand.Meetings.Models;
using YourBrand.Meetings.Domain;
 
namespace YourBrand.Meetings.Features.Groups.Queries;

public record GetMeetingGroupById(string OrganizationId, int Id) : IRequest<Result<MeetingGroupDto>>
{
    public class Handler(IApplicationDbContext context) : IRequestHandler<GetMeetingGroupById, Result<MeetingGroupDto>>
    {
        public async Task<Result<MeetingGroupDto>> Handle(GetMeetingGroupById request, CancellationToken cancellationToken)
        {
            var meetingGroup = await context.MeetingGroups
                .InOrganization(request.OrganizationId)
                .AsNoTracking()
                .Include(x => x.Members.OrderBy(x => x.Order))
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if(meetingGroup is null) 
            {
                return Errors.MeetingGroups.MeetingGroupNotFound;
            }

            return meetingGroup.ToDto();
        }
    }
}
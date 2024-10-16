using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;

namespace YourBrand.Meetings.Features.Command;

public record AddAttendeesFromGroup(string OrganizationId, int Id, int GroupId) : IRequest<Result<MeetingDto>>
{
    public class Validator : AbstractValidator<AddAttendeesFromGroup>
    {
        public Validator()
        {
           //RuleFor(x => x.Name).NotEmpty().MaximumLength(60);
        }
    }

    public class Handler(IApplicationDbContext context) : IRequestHandler<AddAttendeesFromGroup, Result<MeetingDto>>
    {
        public async Task<Result<MeetingDto>> Handle(AddAttendeesFromGroup request, CancellationToken cancellationToken)
        {
            var meeting = await context.Meetings
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (meeting is null)
            {
                return Errors.Meetings.MeetingNotFound;
            }

            var meetingGroup = await context.MeetingGroups
                .InOrganization(request.OrganizationId)
                .Include(x => x.Members.OrderBy(x => x.Order ))
                .FirstOrDefaultAsync(x => x.Id == request.GroupId);

            if (meetingGroup is null)
            {
                return Errors.MeetingGroups.MeetingGroupNotFound;
            }

            foreach(var member in meetingGroup.Members) 
            {
                var attendee = meeting.AddAttendee(member.Name, member.UserId, member.Email, member.Role, member.HasSpeakingRights, member.HasVotingRights, member.MeetingGroupId, member.Id);
            }

            context.Meetings.Update(meeting);

            await context.SaveChangesAsync(cancellationToken);
            
            return meeting.ToDto();
        }
    }
}
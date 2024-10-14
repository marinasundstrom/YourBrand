using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;

namespace YourBrand.Meetings.Features.Groups.Command;

public record RemoveMember(string OrganizationId, int Id, string MemberId) : IRequest<Result<MeetingGroupDto>>
{
    public class Validator : AbstractValidator<RemoveMember>
    {
        public Validator()
        {

        }
    }

    public class Handler(IApplicationDbContext context) : IRequestHandler<RemoveMember, Result<MeetingGroupDto>>
    {
        public async Task<Result<MeetingGroupDto>> Handle(RemoveMember request, CancellationToken cancellationToken)
        {
            var meetingGroup = await context.MeetingGroups
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (meetingGroup is null)
            {
                return Errors.MeetingGroups.MeetingGroupNotFound;
            }

            var member = meetingGroup.Members.FirstOrDefault(x => x.Id == request.MemberId);

            if (member is null)
            {
                return Errors.MeetingGroups.MeetingGroupMemberNotFound;
            }

            meetingGroup.RemoveMember(member);

            context.MeetingGroups.Update(meetingGroup);

            await context.SaveChangesAsync(cancellationToken);

            meetingGroup = await context.MeetingGroups
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == meetingGroup.Id!, cancellationToken);

            if (meetingGroup is null)
            {
                return Errors.MeetingGroups.MeetingGroupNotFound;
            }

            return meetingGroup.ToDto();
        }
    }
}
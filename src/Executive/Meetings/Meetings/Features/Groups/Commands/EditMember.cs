using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;

namespace YourBrand.Meetings.Features.Groups.Command;

public record EditMember(string OrganizationId, int Id, string MemberId, string Name, string? UserId, string Email, ParticipantRole Role, bool HasVotingRights) : IRequest<Result<MeetingGroupMemberDto>>
{
    public class Validator : AbstractValidator<EditMember>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(60);
        }
    }

    public class Handler(IApplicationDbContext context) : IRequestHandler<EditMember, Result<MeetingGroupMemberDto>>
    {
        public async Task<Result<MeetingGroupMemberDto>> Handle(EditMember request, CancellationToken cancellationToken)
        {
            var meetingGroup = await context.MeetingGroups
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (meetingGroup is null)
            {
                return Errors.MeetingGroups.MeetingGroupNotFound;
            }

            var member = meetingGroup.Members.FirstOrDefault(x => x.Id == request.MemberId);

            if(member is  null) 
            {
                return Errors.MeetingGroups.MeetingGroupMemberNotFound;
            }
        
            member.Name = request.Name;
            member.UserId = request.UserId;
            member.Email = request.Email;
            member.Role = request.Role;
            member.HasVotingRights = request.HasVotingRights;

            context.MeetingGroups.Update(meetingGroup);

            await context.SaveChangesAsync(cancellationToken);

            return member.ToDto();
        }
    }
}
using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;

namespace YourBrand.Meetings.Features.Groups.Command;

public record AddMember(string OrganizationId, int Id, string Name, string? UserId, string Email, AttendeeRole Role, bool HasSpeakingRights, bool HasVotingRights) : IRequest<Result<MeetingGroupMemberDto>>
{
    public class Validator : AbstractValidator<AddMember>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(60);
        }
    }

    public class Handler(IApplicationDbContext context) : IRequestHandler<AddMember, Result<MeetingGroupMemberDto>>
    {
        public async Task<Result<MeetingGroupMemberDto>> Handle(AddMember request, CancellationToken cancellationToken)
        {
            var meetingGroup = await context.MeetingGroups
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (meetingGroup is null)
            {
                return Errors.MeetingGroups.MeetingGroupNotFound;
            }

            var member = meetingGroup.AddMember(request.Name, request.Email, request.Role, request.UserId, request.HasSpeakingRights, request.HasVotingRights);

            context.MeetingGroups.Update(meetingGroup);

            await context.SaveChangesAsync(cancellationToken);
            
            return member.ToDto();
        }
    }
}
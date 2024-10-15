using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;

namespace YourBrand.Meetings.Features.Groups.Command;

public sealed record CreateMeetingGroupMemberDto(string Name, string? UserId, string Email, AttendeeRole Role, bool HasSpeakingRights, bool HasVotingRights);

public sealed record CreateMeetingGroupQuorumDto(int RequiredNumber);

public record CreateMeetingGroup(string OrganizationId, string Title, string Description, CreateMeetingGroupQuorumDto Quorum, IEnumerable<CreateMeetingGroupMemberDto> Members) : IRequest<Result<MeetingGroupDto>>
{
    public class Validator : AbstractValidator<CreateMeetingGroup>
    {
        public Validator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(60);

            RuleFor(x => x.Members).NotEmpty();
        }
    }

    public class Handler(IApplicationDbContext context) : IRequestHandler<CreateMeetingGroup, Result<MeetingGroupDto>>
    {
        public async Task<Result<MeetingGroupDto>> Handle(CreateMeetingGroup request, CancellationToken cancellationToken)
        {
            int id = 1;

            try
            {
                id = await context.MeetingGroups
                    .InOrganization(request.OrganizationId)
                    .MaxAsync(x => x.Id, cancellationToken) + 1;
            }
            catch { }

            var meetingGroup = new MeetingGroup(id, request.Title, request.Description);
            meetingGroup.OrganizationId = request.OrganizationId;
            meetingGroup.Quorum.RequiredNumber = request.Quorum.RequiredNumber;

            foreach (var member in request.Members) 
            {
                meetingGroup.AddMember(member.Name, member.Email, member.Role, member.UserId, member.HasSpeakingRights, member.HasVotingRights);
            }

            context.MeetingGroups.Add(meetingGroup);

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
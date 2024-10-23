using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Meetings.Features.Procedure;

namespace YourBrand.Meetings.Features.Groups.Command;

public record ReorderMember(string OrganizationId, int Id, string MemberId, int Order) : IRequest<Result<MeetingGroupMemberDto>>
{
    public class Validator : AbstractValidator<ReorderMember>
    {
        public Validator()
        {
            // RuleFor(x => x.Title).NotEmpty().MaximumLength(60);
        }
    }

    public class Handler(IApplicationDbContext context) : IRequestHandler<ReorderMember, Result<MeetingGroupMemberDto>>
    {
        public async Task<Result<MeetingGroupMemberDto>> Handle(ReorderMember request, CancellationToken cancellationToken)
        {
            var meetingGroup = await context.MeetingGroups
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (meetingGroup is null)
            {
                return Errors.MeetingGroups.MeetingGroupNotFound;
            }

            var member = meetingGroup.GetMemberById(request.MemberId);

            if (member is null)
            {
                return Errors.MeetingGroups.MeetingGroupMemberNotFound;
            }

            meetingGroup.ReorderMember(member, request.Order);

            context.MeetingGroups.Update(meetingGroup);

            await context.SaveChangesAsync(cancellationToken);

            meetingGroup = await context.MeetingGroups
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == meetingGroup.Id!, cancellationToken);

            if (meetingGroup is null)
            {
                return Errors.MeetingGroups.MeetingGroupNotFound;
            }

            return member.ToDto();
        }
    }
}
using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;

namespace YourBrand.Meetings.Features.Groups.Command;

public record ChangeMeetingGroupQuorum(string OrganizationId, int Id, int RequiredNumber) : IRequest<Result<MeetingGroupDto>>
{
    public class Validator : AbstractValidator<ChangeMeetingGroupQuorum>
    {
        public Validator()
        {

        }
    }

    public class Handler(IApplicationDbContext context) : IRequestHandler<ChangeMeetingGroupQuorum, Result<MeetingGroupDto>>
    {
        public async Task<Result<MeetingGroupDto>> Handle(ChangeMeetingGroupQuorum request, CancellationToken cancellationToken)
        {
            var meetingGroup = await context.MeetingGroups
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (meetingGroup is null)
            {
                return Errors.MeetingGroups.MeetingGroupNotFound;
            }

            meetingGroup.Quorum.RequiredNumber = request.RequiredNumber;

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

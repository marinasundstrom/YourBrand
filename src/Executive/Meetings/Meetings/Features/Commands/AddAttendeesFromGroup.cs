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
                .Include(x => x.Members.OrderBy(x => x.Order))
                .FirstOrDefaultAsync(x => x.Id == request.GroupId);

            if (meetingGroup is null)
            {
                return Errors.MeetingGroups.MeetingGroupNotFound;
            }

            await meeting.AddAttendeesFromGroup(meetingGroup, context, cancellationToken);

            context.Meetings.Update(meeting);

            await context.SaveChangesAsync(cancellationToken);

            return meeting.ToDto();
        }
    }
}
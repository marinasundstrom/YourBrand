using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;

namespace YourBrand.Meetings.Features.Groups.Command;

public record UpdateMeetingGroupTitle(string OrganizationId, int Id, string Title) : IRequest<Result<MeetingGroupDto>>
{
    public class Validator : AbstractValidator<UpdateMeetingGroupTitle>
    {
        public Validator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(60);
        }
    }

    public class Handler(IApplicationDbContext context) : IRequestHandler<UpdateMeetingGroupTitle, Result<MeetingGroupDto>>
    {
        public async Task<Result<MeetingGroupDto>> Handle(UpdateMeetingGroupTitle request, CancellationToken cancellationToken)
        {
            var meetingGroup = await context.MeetingGroups
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (meetingGroup is null)
            {
                return Errors.MeetingGroups.MeetingGroupNotFound;
            }

            meetingGroup.Title = request.Title;

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

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;

namespace YourBrand.Meetings.Features.Groups.Command;

public record UpdateMeetingGroupDescription(string OrganizationId, int Id, string Description) : IRequest<Result<MeetingGroupDto>>
{
    public class Validator : AbstractValidator<UpdateMeetingGroupDescription>
    {
        public Validator()
        {
            RuleFor(x => x.Description).NotEmpty().MaximumLength(60);
        }
    }

    public class Handler(IApplicationDbContext context) : IRequestHandler<UpdateMeetingGroupDescription, Result<MeetingGroupDto>>
    {
        public async Task<Result<MeetingGroupDto>> Handle(UpdateMeetingGroupDescription request, CancellationToken cancellationToken)
        {
            var meetingGroup = await context.MeetingGroups
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (meetingGroup is null)
            {
                return Errors.MeetingGroups.MeetingGroupNotFound;
            }

            meetingGroup.Description = request.Description;

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

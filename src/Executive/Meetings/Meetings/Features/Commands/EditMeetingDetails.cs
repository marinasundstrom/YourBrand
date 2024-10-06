using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;

namespace YourBrand.Meetings.Features.Command;

public sealed record EditMeetingDetailsParticipantDto(string Name, string? UserId, string Email, ParticipantRole Role, bool HasVotingRights);

public sealed record EditMeetingDetailsQuorumDto(int RequiredNumber);

public record EditMeetingDetails(string OrganizationId, int Id, string Title, DateTimeOffset? ScheduledAt, string Location, EditMeetingDetailsQuorumDto Quorum) : IRequest<Result<MeetingDto>>
{
    public class Validator : AbstractValidator<EditMeetingDetails>
    {
        public Validator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(60);
        }
    }

    public class Handler(IApplicationDbContext context) : IRequestHandler<EditMeetingDetails, Result<MeetingDto>>
    {
        public async Task<Result<MeetingDto>> Handle(EditMeetingDetails request, CancellationToken cancellationToken)
        {
            var meeting = await context.Meetings
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (meeting is null)
            {
                return Errors.Meetings.MeetingNotFound;
            }

            meeting.Title = request.Title;
            meeting.ScheduledAt = request.ScheduledAt.GetValueOrDefault();
            meeting.Location = request.Location ?? string.Empty;
            meeting.Quorum.RequiredNumber = request.Quorum.RequiredNumber;

            context.Meetings.Update(meeting);

            await context.SaveChangesAsync(cancellationToken);

            meeting = await context.Meetings
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == meeting.Id!, cancellationToken);

            if (meeting is null)
            {
                return Errors.Meetings.MeetingNotFound;
            }

            return meeting.ToDto();
        }
    }
}
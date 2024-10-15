using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;

namespace YourBrand.Meetings.Features.Command;

public sealed record CreateMeetingAttendeeDto(string Name, string? UserId, string Email, AttendeeRole Role, bool HasSpeakingRights, bool HasVotingRights);

public sealed record CreateMeetingQuorumDto(int RequiredNumber);

public record CreateMeeting(string OrganizationId, string Title, string Description, DateTimeOffset? ScheduledAt, string Location, CreateMeetingQuorumDto Quorum, IEnumerable<CreateMeetingAttendeeDto> Attendees) : IRequest<Result<MeetingDto>>
{
    public class Validator : AbstractValidator<CreateMeeting>
    {
        public Validator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(60);

            RuleFor(x => x.Attendees).NotEmpty();
        }
    }

    public class Handler(IApplicationDbContext context) : IRequestHandler<CreateMeeting, Result<MeetingDto>>
    {
        public async Task<Result<MeetingDto>> Handle(CreateMeeting request, CancellationToken cancellationToken)
        {
            int id = 1;

            try
            {
                id = await context.Meetings
                    .InOrganization(request.OrganizationId)
                    .MaxAsync(x => x.Id, cancellationToken) + 1;
            }
            catch { }

            var meeting = new Meeting(id, request.Title);
            meeting.OrganizationId = request.OrganizationId;
            meeting.Location = request.Location ?? string.Empty;
            meeting.Quorum.RequiredNumber = request.Quorum.RequiredNumber;

            if(request.ScheduledAt is not null) 
            {
                meeting.ScheduledAt = request.ScheduledAt.GetValueOrDefault();
            }

            foreach (var attendee in request.Attendees) 
            {
                meeting.AddAttendee(attendee.Name, attendee.UserId, attendee.Email, attendee.Role, attendee.HasSpeakingRights, attendee.HasVotingRights);
            }

            context.Meetings.Add(meeting);

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
using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;

namespace YourBrand.Meetings.Features.Command;

public sealed record CreateMeetingParticipantDto(string Name, string? UserId, string Email, ParticipantRole Role, bool HasVotingRights);

public sealed record CreateMeetingQuorumDto(int RequiredNumber);

public record CreateMeeting(string OrganizationId, string Title, DateTimeOffset? ScheduledAt, string Location, CreateMeetingQuorumDto Quorum, IEnumerable<CreateMeetingParticipantDto> Participants) : IRequest<Result<MeetingDto>>
{
    public class Validator : AbstractValidator<CreateMeeting>
    {
        public Validator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(60);

            RuleFor(x => x.Participants).NotEmpty();
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

            foreach (var participant in request.Participants) 
            {
                meeting.AddParticipant(participant.Name, participant.UserId, participant.Email, participant.Role, participant.HasVotingRights);
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
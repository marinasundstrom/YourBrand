using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;

namespace YourBrand.Meetings.Features.Command;

public record RemoveParticipant(string OrganizationId, int Id, string ParticipantId) : IRequest<Result<MeetingDto>>
{
    public class Validator : AbstractValidator<RemoveParticipant>
    {
        public Validator()
        {

        }
    }

    public class Handler(IApplicationDbContext context) : IRequestHandler<RemoveParticipant, Result<MeetingDto>>
    {
        public async Task<Result<MeetingDto>> Handle(RemoveParticipant request, CancellationToken cancellationToken)
        {
            var meeting = await context.Meetings
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (meeting is null)
            {
                return Errors.Meetings.MeetingNotFound;
            }

            var participant = meeting.Participants.FirstOrDefault(x => x.Id == request.ParticipantId);

            if (participant is null)
            {
                return Errors.Meetings.ParticipantNotFound;
            }

            meeting.RemoveParticipant(participant);

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
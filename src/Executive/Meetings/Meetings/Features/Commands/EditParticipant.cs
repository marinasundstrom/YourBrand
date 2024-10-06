using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;

namespace YourBrand.Meetings.Features.Command;

public record EditParticipant(string OrganizationId, int Id, string ParticipantId, string Name, string? UserId, string Email, ParticipantRole Role, bool HasVotingRights) : IRequest<Result<MeetingDto>>
{
    public class Validator : AbstractValidator<EditParticipant>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(60);
        }
    }

    public class Handler(IApplicationDbContext context) : IRequestHandler<EditParticipant, Result<MeetingDto>>
    {
        public async Task<Result<MeetingDto>> Handle(EditParticipant request, CancellationToken cancellationToken)
        {
            var meeting = await context.Meetings
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (meeting is null)
            {
                return Errors.Meetings.MeetingNotFound;
            }

            var participant = meeting.Participants.FirstOrDefault(x => x.Id == request.ParticipantId);

            if(participant is  null) 
            {
                return Errors.Meetings.ParticipantNotFound;
            }
        
            participant.Name = request.Name;
            participant.UserId = request.UserId;
            participant.Email = request.Email;
            participant.Role = request.Role;
            participant.HasVotingRights = request.HasVotingRights;

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
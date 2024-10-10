using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;

namespace YourBrand.Meetings.Features.Command;

public record MarkParticipantAsPresent(string OrganizationId, int Id, string ParticipantId, bool IsPresent = true) : IRequest<Result<MeetingParticipantDto>>
{
    public class Validator : AbstractValidator<MarkParticipantAsPresent>
    {
        public Validator()
        {
            // RuleFor(x => x.Name).NotEmpty().MaximumLength(60);
        }
    }

    public class Handler(IApplicationDbContext context) : IRequestHandler<MarkParticipantAsPresent, Result<MeetingParticipantDto>>
    {
        public async Task<Result<MeetingParticipantDto>> Handle(MarkParticipantAsPresent request, CancellationToken cancellationToken)
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
        
            participant.IsPresent = request.IsPresent;

            context.Meetings.Update(meeting);

            await context.SaveChangesAsync(cancellationToken);

            return participant.ToDto();
        }
    }
}
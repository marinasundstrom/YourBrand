using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;

namespace YourBrand.Meetings.Features.Command;

public record AddParticipant(string OrganizationId, int Id, string Name, string? UserId, string Email, ParticipantRole Role, bool HasVotingRights) : IRequest<Result<MeetingParticipantDto>>
{
    public class Validator : AbstractValidator<AddParticipant>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(60);
        }
    }

    public class Handler(IApplicationDbContext context) : IRequestHandler<AddParticipant, Result<MeetingParticipantDto>>
    {
        public async Task<Result<MeetingParticipantDto>> Handle(AddParticipant request, CancellationToken cancellationToken)
        {
            var meeting = await context.Meetings
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (meeting is null)
            {
                return Errors.Meetings.MeetingNotFound;
            }

            var participant = meeting.AddParticipant(request.Name, request.UserId, request.Email, request.Role, request.HasVotingRights);

            context.Meetings.Update(meeting);

            await context.SaveChangesAsync(cancellationToken);
            
            return participant.ToDto();
        }
    }
}
using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;

namespace YourBrand.Meetings.Features.Command;

public record AddAttendee(string OrganizationId, int Id, string Name, string? UserId, string Email, AttendeeRole Role, bool HasSpeakingRights, bool HasVotingRights) : IRequest<Result<MeetingAttendeeDto>>
{
    public class Validator : AbstractValidator<AddAttendee>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(60);
        }
    }

    public class Handler(IApplicationDbContext context) : IRequestHandler<AddAttendee, Result<MeetingAttendeeDto>>
    {
        public async Task<Result<MeetingAttendeeDto>> Handle(AddAttendee request, CancellationToken cancellationToken)
        {
            var meeting = await context.Meetings
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (meeting is null)
            {
                return Errors.Meetings.MeetingNotFound;
            }

            var attendee = meeting.AddAttendee(request.Name, request.UserId, request.Email, request.Role, request.HasSpeakingRights, request.HasVotingRights);

            context.Meetings.Update(meeting);

            await context.SaveChangesAsync(cancellationToken);
            
            return attendee.ToDto();
        }
    }
}
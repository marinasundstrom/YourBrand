using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;

namespace YourBrand.Meetings.Features.Command;

public record EditAttendee(string OrganizationId, int Id, string AttendeeId, string Name, string? UserId, string Email, AttendeeRole Role, bool HasSpeakingRights, bool HasVotingRights) : IRequest<Result<MeetingAttendeeDto>>
{
    public class Validator : AbstractValidator<EditAttendee>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(60);
        }
    }

    public class Handler(IApplicationDbContext context) : IRequestHandler<EditAttendee, Result<MeetingAttendeeDto>>
    {
        public async Task<Result<MeetingAttendeeDto>> Handle(EditAttendee request, CancellationToken cancellationToken)
        {
            var meeting = await context.Meetings
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (meeting is null)
            {
                return Errors.Meetings.MeetingNotFound;
            }

            var attendee = meeting.Attendees.FirstOrDefault(x => x.Id == request.AttendeeId);

            if(attendee is  null) 
            {
                return Errors.Meetings.AttendeeNotFound;
            }
        
            attendee.Name = request.Name;
            attendee.UserId = request.UserId;
            attendee.Email = request.Email;
            attendee.Role = request.Role;
            attendee.HasSpeakingRights = request.HasSpeakingRights;
            attendee.HasVotingRights = request.HasVotingRights;

            context.Meetings.Update(meeting);

            await context.SaveChangesAsync(cancellationToken);

            return attendee.ToDto();
        }
    }
}
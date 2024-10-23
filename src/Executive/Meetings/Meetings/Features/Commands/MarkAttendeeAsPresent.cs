using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;

namespace YourBrand.Meetings.Features.Command;

public record MarkAttendeeAsPresent(string OrganizationId, int Id, string AttendeeId, bool IsPresent = true) : IRequest<Result<MeetingAttendeeDto>>
{
    public class Validator : AbstractValidator<MarkAttendeeAsPresent>
    {
        public Validator()
        {
            // RuleFor(x => x.Name).NotEmpty().MaximumLength(60);
        }
    }

    public class Handler(IApplicationDbContext context) : IRequestHandler<MarkAttendeeAsPresent, Result<MeetingAttendeeDto>>
    {
        public async Task<Result<MeetingAttendeeDto>> Handle(MarkAttendeeAsPresent request, CancellationToken cancellationToken)
        {
            var meeting = await context.Meetings
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (meeting is null)
            {
                return Errors.Meetings.MeetingNotFound;
            }

            var attendee = meeting.GetAttendeeById(request.AttendeeId);

            if (attendee is null)
            {
                return Errors.Meetings.AttendeeNotFound;
            }

            attendee.IsPresent = request.IsPresent;

            context.Meetings.Update(meeting);

            await context.SaveChangesAsync(cancellationToken);

            return attendee.ToDto();
        }
    }
}
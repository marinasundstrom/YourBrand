using FluentValidation;
using YourBrand.Meetings.Domain.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;

namespace YourBrand.Meetings.Features.Procedure.Command;

public sealed record AdjournMeeting(string OrganizationId, int Id, string Message) : IRequest<Result>
{
    public sealed class Validator : AbstractValidator<AdjournMeeting>
    {
        public Validator()
        {
            RuleFor(x => x.Message).NotEmpty();
        }
    }

    public sealed class Handler(IApplicationDbContext context, IUserContext userContext) : IRequestHandler<AdjournMeeting, Result>
    {
        public async Task<Result> Handle(AdjournMeeting request, CancellationToken cancellationToken)
        {
            var meeting = await context.Meetings
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (meeting is null)
            {
                return Errors.Meetings.MeetingNotFound;
            }

            var attendee = meeting.GetAttendeeByUserId(userContext.UserId);

            if (attendee is null)
            {
                return Errors.Meetings.YouAreNotAttendeeOfMeeting;
            }

            var chairFunction = meeting.GetChairpersonFunction(attendee);

            if (chairFunction is null)
            {
                return Errors.Meetings.OnlyChairpersonCanAdjournTheMeeting;
            }

            chairFunction.AdjournMeeting(request.Message.Trim());

            context.Meetings.Update(meeting);

            await context.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}

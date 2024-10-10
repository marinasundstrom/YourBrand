using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;

namespace YourBrand.Meetings.Features.Procedure.Command;

public record ResetMeetingProcedure(string OrganizationId, int Id) : IRequest<Result<MeetingDto>>
{
    public class Validator : AbstractValidator<ResetMeetingProcedure>
    {
        public Validator()
        {
            //RuleFor(x => x.Title).NotEmpty().MaximumLength(60);
        }
    }

    public class Handler(IApplicationDbContext context) : IRequestHandler<ResetMeetingProcedure, Result<MeetingDto>>
    {
        public async Task<Result<MeetingDto>> Handle(ResetMeetingProcedure request, CancellationToken cancellationToken)
        {
            var meeting = await context.Meetings
                .InOrganization(request.OrganizationId)
                .Include(x => x.Agenda)
                .ThenInclude(x => x.Items.OrderBy(x => x.Order))
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (meeting is null)
            {
                return Errors.Meetings.MeetingNotFound;
            }

            meeting.ResetProcedure();

            context.Meetings.Update(meeting);

            await context.SaveChangesAsync(cancellationToken);

            return meeting.ToDto();
        }
    }
}
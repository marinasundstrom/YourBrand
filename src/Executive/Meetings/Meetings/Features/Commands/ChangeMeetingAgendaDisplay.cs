using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;

namespace YourBrand.Meetings.Features.Command;

public sealed record ChangeMeetingAgendaDisplay(string OrganizationId, int Id, bool ShowAgendaTimeEstimates) : IRequest<Result<MeetingDto>>
{
    public sealed class Validator : AbstractValidator<ChangeMeetingAgendaDisplay>
    {
        public Validator()
        {
        }
    }

    public sealed class Handler(IApplicationDbContext context) : IRequestHandler<ChangeMeetingAgendaDisplay, Result<MeetingDto>>
    {
        public async Task<Result<MeetingDto>> Handle(ChangeMeetingAgendaDisplay request, CancellationToken cancellationToken)
        {
            var meeting = await context.Meetings
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (meeting is null)
            {
                return Errors.Meetings.MeetingNotFound;
            }

            meeting.ShowAgendaTimeEstimates = request.ShowAgendaTimeEstimates;

            context.Meetings.Update(meeting);

            await context.SaveChangesAsync(cancellationToken);

            meeting = await context.Meetings
                .InOrganization(request.OrganizationId)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == meeting.Id, cancellationToken);

            if (meeting is null)
            {
                return Errors.Meetings.MeetingNotFound;
            }

            return meeting.ToDto();
        }
    }
}

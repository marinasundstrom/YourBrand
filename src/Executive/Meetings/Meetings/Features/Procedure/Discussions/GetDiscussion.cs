using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Meetings.Features.Agendas;

namespace YourBrand.Meetings.Features.Procedure.Discussions;

public sealed record GetDiscussion(string OrganizationId, int Id) : IRequest<Result<DiscussionDto?>>
{
    public sealed class Handler(IApplicationDbContext context, IUserContext userContext) : IRequestHandler<GetDiscussion, Result<DiscussionDto?>>
    {
        public async Task<Result<DiscussionDto?>> Handle(GetDiscussion request, CancellationToken cancellationToken)
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

            var agendaItem = meeting.GetCurrentAgendaItem();

            if (agendaItem is null)
            {
                return Errors.Meetings.NoActiveAgendaItem;
            }

            if (agendaItem.Discussion is null)
            {
                return Errors.Meetings.NoOngoingDiscussionSession;
            }

            return agendaItem.Discussion?.ToDto();
        }
    }
}
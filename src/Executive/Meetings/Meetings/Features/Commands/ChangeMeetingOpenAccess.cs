using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Meetings.Domain.Entities;

namespace YourBrand.Meetings.Features.Command;

public sealed record ChangeMeetingOpenAccess(string OrganizationId, int Id, bool CanAnyoneJoin, int? JoinAsRoleId) : IRequest<Result<MeetingDto>>
{
    public sealed class Handler(IApplicationDbContext context) : IRequestHandler<ChangeMeetingOpenAccess, Result<MeetingDto>>
    {
        public async Task<Result<MeetingDto>> Handle(ChangeMeetingOpenAccess request, CancellationToken cancellationToken)
        {
            var meeting = await context.Meetings
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (meeting is null)
            {
                return Errors.Meetings.MeetingNotFound;
            }

            var joinRoleId = request.JoinAsRoleId ?? meeting.JoinAsId;

            var joinRole = await context.AttendeeRoles.FirstOrDefaultAsync(x => x.Id == joinRoleId, cancellationToken);

            if (joinRole is null)
            {
                throw new Exception("Invalid role");
            }

            if (request.CanAnyoneJoin && joinRole.Id != AttendeeRole.Member.Id && joinRole.Id != AttendeeRole.Observer.Id)
            {
                return Errors.Meetings.InvalidOpenAccessRole;
            }

            meeting.SetOpenAccess(request.CanAnyoneJoin, joinRole);

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

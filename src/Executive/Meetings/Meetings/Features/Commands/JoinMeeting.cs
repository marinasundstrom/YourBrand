using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Meetings.Domain.Entities;

namespace YourBrand.Meetings.Features.Command;

public sealed record JoinMeeting(string OrganizationId, int Id) : IRequest<Result<MeetingDto>>
{
    public sealed class Handler(IApplicationDbContext context, IUserContext userContext) : IRequestHandler<JoinMeeting, Result<MeetingDto>>
    {
        public async Task<Result<MeetingDto>> Handle(JoinMeeting request, CancellationToken cancellationToken)
        {
            var meeting = await context.Meetings
                .InOrganization(request.OrganizationId)
                .Include(x => x.Attendees.OrderBy(x => x.Order))
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (meeting is null)
            {
                return Errors.Meetings.MeetingNotFound;
            }

            if (!meeting.CanAnyoneJoin)
            {
                return Errors.Meetings.MeetingNotOpenForGuests;
            }

            var userId = userContext.UserId?.Value;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Errors.Users.UserNotFound;
            }

            var joinRole = meeting.JoinAs;

            if (joinRole is null)
            {
                joinRole = await context.AttendeeRoles.FirstOrDefaultAsync(x => x.Id == meeting.JoinAsId, cancellationToken)
                    ?? throw new Exception("Invalid role");
            }

            if (meeting.CanAnyoneJoin && joinRole.Id != AttendeeRole.Member.Id && joinRole.Id != AttendeeRole.Observer.Id)
            {
                return Errors.Meetings.InvalidOpenAccessRole;
            }

            var displayName = BuildDisplayName(userContext, userId);
            var email = string.IsNullOrWhiteSpace(userContext.Email) ? $"{userId}@example.com" : userContext.Email!;
            var now = DateTimeOffset.UtcNow;

            var attendee = meeting.GetAttendeeByUserId(userId);

            if (attendee is null)
            {
                attendee = meeting.AddAttendee(displayName, userId, email, joinRole, joinRole.CanSpeak, joinRole.CanVote, Enumerable.Empty<MeetingFunction>());
            }
            else
            {
                attendee.RemovedAt = null;
                attendee.Name = displayName;
                attendee.Email = email;
                attendee.Role = joinRole;
                attendee.RoleId = joinRole.Id;
                attendee.HasSpeakingRights = joinRole.CanSpeak;
                attendee.HasVotingRights = joinRole.CanVote;
                attendee.SetFunctions(Array.Empty<MeetingFunction>());
            }

            attendee.JoinedAt = now;
            attendee.IsPresent = true;

            context.Meetings.Update(meeting);

            await context.SaveChangesAsync(cancellationToken);

            meeting = await context.Meetings
                .InOrganization(request.OrganizationId)
                .Include(x => x.Attendees.OrderBy(x => x.Order))
                .FirstOrDefaultAsync(x => x.Id == meeting.Id!, cancellationToken);

            if (meeting is null)
            {
                return Errors.Meetings.MeetingNotFound;
            }

            return meeting.ToDto();
        }

        private static string BuildDisplayName(IUserContext userContext, string fallback)
        {
            var parts = new List<string>();

            if (!string.IsNullOrWhiteSpace(userContext.FirstName))
            {
                parts.Add(userContext.FirstName!);
            }

            if (!string.IsNullOrWhiteSpace(userContext.LastName))
            {
                parts.Add(userContext.LastName!);
            }

            if (parts.Count > 0)
            {
                return string.Join(" ", parts);
            }

            if (!string.IsNullOrWhiteSpace(userContext.Email))
            {
                return userContext.Email!;
            }

            return fallback;
        }
    }
}

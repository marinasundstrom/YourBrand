using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Meetings.Domain.Entities;

namespace YourBrand.Meetings.Features.Command;

public sealed record CreateMeetingAttendeeDto(string Name, string? UserId, string Email, int Role, bool? HasSpeakingRights, bool? HasVotingRights, IEnumerable<int>? FunctionIds);

public sealed record CreateMeetingQuorumDto(int RequiredNumber);

public record CreateMeeting(string OrganizationId, string Title, string Description, DateTimeOffset? ScheduledAt, string Location, CreateMeetingQuorumDto Quorum, IEnumerable<CreateMeetingAttendeeDto> Attendees, bool CanAnyoneJoin, int? JoinAsRoleId) : IRequest<Result<MeetingDto>>
{
    public class Validator : AbstractValidator<CreateMeeting>
    {
        public Validator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(60);

            RuleFor(x => x.Attendees).NotEmpty();
        }
    }

    public class Handler(IApplicationDbContext context) : IRequestHandler<CreateMeeting, Result<MeetingDto>>
    {
        public async Task<Result<MeetingDto>> Handle(CreateMeeting request, CancellationToken cancellationToken)
        {
            int id = 1;

            try
            {
                id = await context.Meetings
                    .InOrganization(request.OrganizationId)
                    .MaxAsync(x => x.Id, cancellationToken) + 1;
            }
            catch { }

            var joinRoleId = request.JoinAsRoleId ?? AttendeeRole.Observer.Id;

            var joinRole = await context.AttendeeRoles.FirstOrDefaultAsync(x => x.Id == joinRoleId, cancellationToken);

            if (joinRole is null)
            {
                throw new Exception("Invalid role");
            }

            if (request.CanAnyoneJoin && joinRole.Id != AttendeeRole.Member.Id && joinRole.Id != AttendeeRole.Observer.Id)
            {
                return Errors.Meetings.InvalidOpenAccessRole;
            }

            var meeting = new Meeting(id, request.Title);
            meeting.OrganizationId = request.OrganizationId;
            meeting.Location = request.Location ?? string.Empty;
            meeting.Quorum.RequiredNumber = request.Quorum.RequiredNumber;
            meeting.SetOpenAccess(request.CanAnyoneJoin, joinRole);

            if (request.ScheduledAt is not null)
            {
                meeting.ScheduledAt = request.ScheduledAt.GetValueOrDefault();
            }

            foreach (var attendee in request.Attendees)
            {
                var role = await context.AttendeeRoles.FirstOrDefaultAsync(x => x.Id == attendee.Role);

                if (role is null)
                {
                    throw new Exception("Invalid role");
                }

                var functionIds = attendee.FunctionIds?.Distinct().ToArray();

                var functions = functionIds is null || functionIds.Length == 0
                    ? new List<MeetingFunction>()
                    : await context.MeetingFunctions
                        .Where(x => functionIds.Contains(x.Id))
                        .ToListAsync(cancellationToken);

                if (functionIds is not null && functions.Count != functionIds.Length)
                {
                    throw new Exception("Invalid meeting function");
                }

                meeting.AddAttendee(attendee.Name, attendee.UserId, attendee.Email, role, attendee.HasSpeakingRights, attendee.HasVotingRights, functions);
            }

            context.Meetings.Add(meeting);

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
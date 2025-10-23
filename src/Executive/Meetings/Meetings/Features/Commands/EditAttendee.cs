using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Meetings.Domain.Entities;

namespace YourBrand.Meetings.Features.Command;

public record EditAttendee(string OrganizationId, int Id, string AttendeeId, string Name, string? UserId, string Email, int Role, bool? HasSpeakingRights, bool? HasVotingRights, IEnumerable<int>? FunctionIds) : IRequest<Result<MeetingAttendeeDto>>
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

            var attendee = meeting.GetAttendeeById(request.AttendeeId);

            if (attendee is null)
            {
                return Errors.Meetings.AttendeeNotFound;
            }

            var role = await context.AttendeeRoles.FirstOrDefaultAsync(x => x.Id == request.Role, cancellationToken);

            if (role is null)
            {
                throw new Exception("Invalid role");
            }

            attendee.Name = request.Name;
            attendee.UserId = request.UserId;
            attendee.Email = request.Email;
            attendee.Role = role;
            attendee.HasSpeakingRights = request.HasSpeakingRights;
            attendee.HasVotingRights = request.HasVotingRights;

            if (request.FunctionIds is not null)
            {
                var functionIds = request.FunctionIds.Distinct().ToArray();

                var functions = await context.MeetingFunctions
                    .Where(x => functionIds.Contains(x.Id))
                    .ToListAsync(cancellationToken);

                if (functions.Count != functionIds.Length)
                {
                    throw new Exception("Invalid meeting function");
                }

                attendee.SetFunctions(functions);
            }

            context.Meetings.Update(meeting);

            await context.SaveChangesAsync(cancellationToken);

            return attendee.ToDto();
        }
    }
}
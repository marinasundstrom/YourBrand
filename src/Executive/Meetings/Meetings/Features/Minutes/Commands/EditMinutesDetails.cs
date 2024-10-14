using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Meetings.Features.Procedure.Command;

namespace YourBrand.Meetings.Features.Minutes.Command;

public sealed record EditMeetingDetailsQuorumDto(int RequiredNumber);

public record EditMinutesDetails(string OrganizationId, int Id) : IRequest<Result<MinutesDto>>
{
    public class Validator : AbstractValidator<EditMinutesDetails>
    {
        public Validator()
        {
            //RuleFor(x => x.Title).NotEmpty().MaximumLength(60);
        }
    }

    public class Handler(IApplicationDbContext context, IHubContext<SecretaryHub, ISecretaryHubClient> hubContext) : IRequestHandler<EditMinutesDetails, Result<MinutesDto>>
    {
        public async Task<Result<MinutesDto>> Handle(EditMinutesDetails request, CancellationToken cancellationToken)
        {
            var minute = await context.Minutes
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (minute is null)
            {
                return Errors.Minutes.MinuteNotFound;
            }

            //minute.Title = request.Title;

            context.Minutes.Update(minute);

            await context.SaveChangesAsync(cancellationToken);

            minute = await context.Minutes
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == minute.Id!, cancellationToken);

            if (minute is null)
            {
                return Errors.Minutes.MinuteNotFound;
            }

            await hubContext.Clients
                .Group($"meeting-{minute.MeetingId}")
                .OnMinutesUpdated();

            return minute.ToDto();
        }
    }
}
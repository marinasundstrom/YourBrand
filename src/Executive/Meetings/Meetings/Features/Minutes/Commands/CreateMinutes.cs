using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;

namespace YourBrand.Meetings.Features.Minutes.Command;

public sealed record CreateMinutesItemDto(int? AgendaId, string? AgendaItemId, AgendaItemType Type, string Title, string Description);

public record CreateMinutes(string OrganizationId, int MeetingId, IEnumerable<CreateMinutesItemDto> Items) : IRequest<Result<MinutesDto>>
{
    public class Validator : AbstractValidator<CreateMinutes>
    {
        public Validator()
        {
            //RuleFor(x => x.Title).NotEmpty().MaximumLength(60);

            //RuleFor(x => x.Items).NotEmpty();
        }
    }

    public class Handler(IApplicationDbContext context) : IRequestHandler<CreateMinutes, Result<MinutesDto>>
    {
        public async Task<Result<MinutesDto>> Handle(CreateMinutes request, CancellationToken cancellationToken)
        {
            int id = 1;

            try
            {
                id = await context.Minutes
                    .InOrganization(request.OrganizationId)
                    .MaxAsync(x => x.Id, cancellationToken) + 1;
            }
            catch { }

            var minute = new Domain.Entities.Minutes(id);
            minute.OrganizationId = request.OrganizationId;
            minute.MeetingId = request.MeetingId;

            foreach (var minuteItem in request.Items) 
            {
                minute.AddItem(minuteItem.Type, minuteItem.AgendaId, minuteItem.AgendaItemId, minuteItem.Title, minuteItem.Description);
            }

            context.Minutes.Add(minute);

            await context.SaveChangesAsync(cancellationToken);

            minute = await context.Minutes
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == minute.Id!, cancellationToken);

            if (minute is null)
            {
                return Errors.Minutes.MinuteNotFound;
            }

            return minute.ToDto();
        }
    }
}
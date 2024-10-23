using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Meetings.Domain;
using YourBrand.Meetings.Features;
using YourBrand.Meetings.Models;

namespace YourBrand.Meetings.Features.Minutes.Queries;

public record GetMinutesById(string OrganizationId, int Id) : IRequest<Result<MinutesDto>>
{
    public class Handler(IApplicationDbContext context) : IRequestHandler<GetMinutesById, Result<MinutesDto>>
    {
        public async Task<Result<MinutesDto>> Handle(GetMinutesById request, CancellationToken cancellationToken)
        {
            var minute = await context.Minutes
                .InOrganization(request.OrganizationId)
                .AsNoTracking()
                .Include(x => x.Items.OrderBy(x => x.Order))
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (minute is null)
            {
                return Errors.Minutes.MinuteNotFound;
            }

            return minute.ToDto();
        }
    }
}
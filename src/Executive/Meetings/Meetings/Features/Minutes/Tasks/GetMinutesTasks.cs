using System.Linq;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Meetings.Features.Minutes;

namespace YourBrand.Meetings.Features.Minutes.Tasks;

public sealed record GetMinutesTasks(string OrganizationId, int MinutesId) : IRequest<IEnumerable<MinutesTaskDto>>;

public sealed class GetMinutesTasksHandler(IApplicationDbContext context) : IRequestHandler<GetMinutesTasks, IEnumerable<MinutesTaskDto>>
{
    public async Task<IEnumerable<MinutesTaskDto>> Handle(GetMinutesTasks request, CancellationToken cancellationToken)
    {
        var tasks = await context.MinutesTasks
            .InOrganization(request.OrganizationId)
            .Where(x => x.MinutesId == request.MinutesId)
            .OrderBy(x => x.Type)
            .ThenBy(x => x.DueAt)
            .ToListAsync(cancellationToken);

        return tasks.Select(x => x.ToDto());
    }
}

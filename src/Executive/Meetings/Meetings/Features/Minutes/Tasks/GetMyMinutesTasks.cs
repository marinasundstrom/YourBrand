using System;
using System.Linq;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Meetings.Features.Minutes;

namespace YourBrand.Meetings.Features.Minutes.Tasks;

public sealed record GetMyMinutesTasks(string OrganizationId, MinutesTaskStatus? Status = null) : IRequest<IEnumerable<MinutesTaskDto>>;

public sealed class GetMyMinutesTasksHandler(IApplicationDbContext context, IUserContext userContext) : IRequestHandler<GetMyMinutesTasks, IEnumerable<MinutesTaskDto>>
{
    public async Task<IEnumerable<MinutesTaskDto>> Handle(GetMyMinutesTasks request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userContext.UserId))
        {
            return Enumerable.Empty<MinutesTaskDto>();
        }

        var query = context.MinutesTasks
            .InOrganization(request.OrganizationId)
            .Where(x => x.AssignedToId == userContext.UserId);

        if (request.Status.HasValue)
        {
            query = query.Where(x => x.Status == request.Status);
        }

        var tasks = await query
            .OrderBy(x => x.DueAt ?? DateTimeOffset.MaxValue)
            .ThenBy(x => x.Type)
            .ToListAsync(cancellationToken);

        return tasks.Select(x => x.ToDto());
    }
}

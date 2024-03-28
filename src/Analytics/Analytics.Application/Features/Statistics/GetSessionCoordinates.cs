using MediatR;
using Microsoft.EntityFrameworkCore;

namespace YourBrand.Analytics.Application.Features.Statistics;

public record GetSessionCoordinates(DateTime? From = null, DateTime? To = null) : IRequest<IEnumerable<SessionCoordinates>>
{
    public class Handler : IRequestHandler<GetSessionCoordinates, IEnumerable<SessionCoordinates>>
    {
        private readonly IApplicationDbContext context;

        public Handler(IApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<SessionCoordinates>> Handle(GetSessionCoordinates request, CancellationToken cancellationToken)
        {
            var sessions = await context.Sessions
                       .Include(x => x.Client)
                       .Where(x => x.Coordinates != null)
                       .OrderByDescending(x => x.Started)
                       .AsNoTracking()
                       .AsSplitQuery()
                       .ToArrayAsync(cancellationToken);

            return sessions.Select(s => new SessionCoordinates(s.Started, s.Client.UserAgent, s.Coordinates!));
        }
    }
}

public record SessionCoordinates(DateTimeOffset DateTime, string UserAgent, Domain.ValueObjects.Coordinates Coordinates);
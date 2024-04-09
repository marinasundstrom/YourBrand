using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Analytics.Application.Features.Tracking;

public record RegisterGeoLocation(string ClientId, string SessionId, YourBrand.Analytics.Domain.ValueObjects.Coordinates Coordinates) : IRequest
{
    public class Handler(IApplicationDbContext context) : IRequestHandler<RegisterGeoLocation>
    {
        public async Task Handle(RegisterGeoLocation request, CancellationToken cancellationToken)
        {
            var session = await context.Sessions
                .FirstAsync(x => x.Id == request.SessionId && x.ClientId == request.ClientId, cancellationToken);

            session.Coordinates = request.Coordinates;

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
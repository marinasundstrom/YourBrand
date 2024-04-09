using MediatR;

using YourBrand.Analytics;
using YourBrand.StoreFront.API;

namespace YourBrand.StoreFront.Application.Features.Analytics;

public sealed record RegisterCoordinates(double Latitude, double Longitude) : IRequest
{
    sealed class Handler(
        YourBrand.Analytics.ISessionClient sessionClient,
        IUserContext userContext) : IRequestHandler<RegisterCoordinates>
    {
        public async Task Handle(RegisterCoordinates request, CancellationToken cancellationToken)
        {
            await sessionClient.RegisterCoordinatesAsync(userContext.ClientId, userContext.SessionId, new Coordinates { Latitude = request.Latitude, Longitude = request.Longitude }, cancellationToken);
        }
    }
}
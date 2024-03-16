using MediatR;
using YourBrand.Analytics;
using YourBrand.StoreFront.API;

namespace YourBrand.StoreFront.Application.Features.Analytics;

public sealed record RegisterCoordinates(double Latitude, double Longitude) : IRequest
{
    sealed class Handler : IRequestHandler<RegisterCoordinates>
    {
        private readonly ISessionClient sessionClient;
        private readonly ICurrentUserService currentUserService;

        public Handler(
            YourBrand.Analytics.ISessionClient sessionClient,
            ICurrentUserService currentUserService)
        {
            this.sessionClient = sessionClient;
            this.currentUserService = currentUserService;
        }

        public async Task Handle(RegisterCoordinates request, CancellationToken cancellationToken)
        {
            await sessionClient.RegisterCoordinatesAsync(currentUserService.ClientId, currentUserService.SessionId, new Coordinates { Latitude = request.Latitude, Longitude = request.Longitude }, cancellationToken);
        }
    }
}
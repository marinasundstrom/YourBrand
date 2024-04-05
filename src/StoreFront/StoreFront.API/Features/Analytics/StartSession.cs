using MediatR;

using YourBrand.Analytics;
using YourBrand.StoreFront.API;

namespace YourBrand.StoreFront.Application.Features.Analytics;

public sealed record StartSession : IRequest<string>
{
    sealed class Handler : IRequestHandler<StartSession, string>
    {
        private readonly ISessionClient sessionClient;
        private readonly IUserContext userContext;

        public Handler(
            YourBrand.Analytics.ISessionClient sessionClient,
            IUserContext userContext)
        {
            this.sessionClient = sessionClient;
            this.userContext = userContext;
        }

        public async Task<string> Handle(StartSession request, CancellationToken cancellationToken)
        {
            return await sessionClient.InitSessionAsync(userContext.ClientId, new YourBrand.Analytics.SessionData() { IpAddress = userContext.GetRemoteIPAddress() }, cancellationToken);
        }
    }
}
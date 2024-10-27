using MediatR;

using YourBrand.Analytics;
using YourBrand.StoreFront.API;

namespace YourBrand.StoreFront.Application.Features.Analytics;

public sealed record StartSession : IRequest<string>
{
    sealed class Handler(
        YourBrand.Analytics.ISessionClient sessionClient,
        IUserContext userContext) : IRequestHandler<StartSession, string>
    {
        public async Task<string> Handle(StartSession request, CancellationToken cancellationToken)
        {
            return await sessionClient.InitSessionAsync(userContext.ClientId, new YourBrand.Analytics.SessionRequestData() { IpAddress = userContext.GetRemoteIPAddress() }, cancellationToken);
        }
    }
}
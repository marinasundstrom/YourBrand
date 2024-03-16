using System;
using System.Net;
using MediatR;
using YourBrand.Analytics;
using YourBrand.StoreFront.API;

namespace YourBrand.StoreFront.Application.Features.Analytics;

public sealed record StartSession : IRequest<string>
{
    sealed class Handler : IRequestHandler<StartSession, string>
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

        public async Task<string> Handle(StartSession request, CancellationToken cancellationToken)
        {
            return await sessionClient.InitSessionAsync(currentUserService.ClientId, new YourBrand.Analytics.SessionData() { IpAddress = currentUserService.GetRemoteIPAddress() }, cancellationToken);
        }
    }
}

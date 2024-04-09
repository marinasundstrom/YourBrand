using MediatR;

using YourBrand.Analytics;
using YourBrand.StoreFront.API;

namespace YourBrand.StoreFront.Application.Features.Analytics;

public sealed record CreateClient : IRequest<string>
{
    sealed class Handler(
        YourBrand.Analytics.IClientClient clientClient,
        IUserContext userContext) : IRequestHandler<CreateClient, string>
    {
        public async Task<string> Handle(CreateClient request, CancellationToken cancellationToken)
        {
            var userAgent = userContext.UserAgent!.ToString();

            return await clientClient.InitClientAsync(new YourBrand.Analytics.ClientData()
            {
                UserAgent = userAgent!
            }, cancellationToken);
        }
    }
}

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.ApiKeys.Application.Common.Interfaces;
using YourBrand.ApiKeys.Domain.Enums;

namespace YourBrand.ApiKeys.Application.Commands;

public record CheckApiKeyCommand(string ApiKey, string Origin, string ServiceSecret, string[] RequestedResources) : IRequest<ApiKeyResult>
{
    public class CheckApiKeyCommandHandler(IApiKeysContext context) : IRequestHandler<CheckApiKeyCommand, ApiKeyResult>
    {
        public async Task<ApiKeyResult> Handle(CheckApiKeyCommand request, CancellationToken cancellationToken)
        {
            Console.WriteLine("Origin: " + request.Origin);

            var apiKey = await context.ApiKeys
                .Include(a => a.Application)
                .Include(a => a.ApiKeyServices)
                .ThenInclude(a => a.Service)
                .AsSplitQuery()
                .AsNoTracking()
                .Where(a => a.ApiKeyServices.Any(s => s.Service.Secret == request.ServiceSecret /* && s.Service.Url == request.Origin */))
                .FirstOrDefaultAsync(apiKey => apiKey.Key == request.ApiKey);

            return apiKey is null
                ? new ApiKeyResult(ApiKeyAuthCode.Unauthorized)
                : new ApiKeyResult(apiKey.Status switch
                {
                    ApiKeyStatus.Active => ApiKeyAuthCode.Authorized,
                    ApiKeyStatus.Expired => ApiKeyAuthCode.Expired,
                    ApiKeyStatus.Revoked => ApiKeyAuthCode.Revoked,
                    _ => throw new Exception()
                }, new ApplicationInfo(apiKey.Application!.Id, apiKey.Application.Name));
        }
    }
}

public record ApiKeyResult(ApiKeyAuthCode Status, ApplicationInfo? Application = null);

public record ApplicationInfo(string Id, string Name);

public enum ApiKeyAuthCode
{
    Authorized,
    Unauthorized,
    Expired,
    Revoked
}
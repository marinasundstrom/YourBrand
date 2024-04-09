using System.Text.Json;

using MediatR;

using YourBrand.Analytics;
using YourBrand.StoreFront.API;

namespace YourBrand.StoreFront.Application.Features.Analytics;

public sealed record RegisterEvent(string ClientId, string SessionId, EventType EventType, IDictionary<string, object> Data) : IRequest<string>
{
    sealed class Handler(
        YourBrand.Analytics.IEventsClient eventsClient,
        IUserContext userContext) : IRequestHandler<RegisterEvent, string>
    {
        public async Task<string> Handle(RegisterEvent request, CancellationToken cancellationToken)
        {
            try
            {
                var data = new Dictionary<string, object>(
                    request.Data.Select(x => new KeyValuePair<string, object>(x.Key, AutoDeserialize((JsonElement)x.Value)!)));

                return await eventsClient.RegisterEventAsync(request.ClientId, request.SessionId,
                    new EventData { EventType = request.EventType, Data = data }, cancellationToken);
            }
            catch (YourBrand.Analytics.ApiException exc) when (exc.StatusCode == 204)
            {

            }

            return null!;
        }

        object? AutoDeserialize(JsonElement e) => e.ValueKind switch
        {
            JsonValueKind.String => e.GetString(),
            JsonValueKind.Null => null,
            JsonValueKind.Number => e.GetDecimal(),
            JsonValueKind.True => e.GetBoolean(),
            JsonValueKind.False => e.GetBoolean(),
            _ => throw new Exception()
        };
    }
}
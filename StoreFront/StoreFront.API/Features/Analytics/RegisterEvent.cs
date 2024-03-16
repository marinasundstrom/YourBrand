using MediatR;
using YourBrand.Analytics;
using YourBrand.StoreFront.API;

using System.Text.Json;

namespace YourBrand.StoreFront.Application.Features.Analytics;

public sealed record RegisterEvent(string ClientId, string SessionId, EventType EventType, IDictionary<string, object> Data) : IRequest<string>
{
    sealed class Handler : IRequestHandler<RegisterEvent, string>
    {
        private IEventsClient eventsClient;
        private readonly ICurrentUserService currentUserService;

        public Handler(
            YourBrand.Analytics.IEventsClient eventsClient,
            ICurrentUserService currentUserService)
        {
            this.eventsClient = eventsClient;
            this.currentUserService = currentUserService;
        }

        public async Task<string> Handle(RegisterEvent request, CancellationToken cancellationToken)
        {
            try
            {
                var data = new Dictionary<string, object>(
                    request.Data.Select(x => new KeyValuePair<string, object>(x.Key, AutoDeserialize((JsonElement)x.Value)!)));
                
                return await eventsClient.RegisterEventAsync(request.ClientId, request.SessionId,
                    new EventData { EventType = request.EventType, Data = data}, cancellationToken);
            }
            catch (YourBrand.Analytics.ApiException exc) when (exc.StatusCode == 204)
            {

            }

            return null!;
        }

        object? AutoDeserialize(JsonElement e) => e.ValueKind switch {
            JsonValueKind.String => e.GetString(),
            JsonValueKind.Null => null,
            JsonValueKind.Number => e.GetDecimal(),
            JsonValueKind.True => e.GetBoolean(),
            JsonValueKind.False => e.GetBoolean(),
            _ => throw new Exception()
        };
    }
}

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using YourBrand.Analytics.Application.Common.Models;
using System.ComponentModel;

namespace YourBrand.Analytics.Application.Features.Statistics;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
public class StatisticsController(IMediator mediator) : ControllerBase
{
    [EndpointSummary("Get most viewed items")]
    [EndpointDescription("Returns the most viewed items over a specific period of time.")]
    [HttpGet("MostViewedItems")]
    public async Task<Data> GetMostViewedItems(DateTime? From = null, DateTime? To = null, bool DistinctByClient = false, CancellationToken cancellationToken = default)
    {
        return await mediator.Send(new GetMostViewedItems(From, To, DistinctByClient), cancellationToken);
    }

    [EndpointSummary("Get sessions count")]
    [EndpointDescription("Returns the number of sessions over a specific period of time.")]
    [HttpGet("GetSessionsCount")]
    public async Task<Data> GetSessionsCount(DateTime? From = null, DateTime? To = null, bool DistinctByClient = false, CancellationToken cancellationToken = default)
    {
        return await mediator.Send(new GetSessionsCount(From, To, DistinctByClient), cancellationToken);
    }

    [EndpointSummary("Get session coordinates")]
    [EndpointDescription("Returns the coordinates for sessions.")]
    [HttpGet("GetSessionCoordinates")]
    public async Task<IEnumerable<SessionCoordinates>> GetSessionCoordinates(DateTime? From = null, DateTime? To = null, CancellationToken cancellationToken = default)
    {
        return await mediator.Send(new GetSessionCoordinates(From, To), cancellationToken);
    }
}
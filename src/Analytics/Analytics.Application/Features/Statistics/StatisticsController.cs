using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace YourBrand.Analytics.Application.Features.Statistics;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
public class StatisticsController : ControllerBase
{
    private readonly IMediator _mediator;

    public StatisticsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("MostViewedItems")]
    public async Task<Data> GetMostViewedItems(DateTime? From = null, DateTime? To = null, bool DistinctByClient = false, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetMostViewedItems(From, To, DistinctByClient), cancellationToken);
    }

    [HttpGet("GetSessionsCount")]
    public async Task<Data> GetSessionsCount(DateTime? From = null, DateTime? To = null, bool DistinctByClient = false, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetSessionsCount(From, To, DistinctByClient), cancellationToken);
    }

    [HttpGet("GetSessionCoordinates")]
    public async Task<IEnumerable<SessionCoordinates>> GetSessionCoordinates(DateTime? From = null, DateTime? To = null, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetSessionCoordinates(From, To), cancellationToken);
    }
}

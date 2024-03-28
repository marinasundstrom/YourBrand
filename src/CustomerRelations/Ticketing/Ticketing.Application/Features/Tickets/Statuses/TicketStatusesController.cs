using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YourBrand.Ticketing.Application;
using YourBrand.Ticketing.Application.Common;
using YourBrand.Ticketing.Application.Features.Tickets.Dtos;
using YourBrand.Ticketing.Application.Features.Tickets.Statuses;

namespace YourBrand.Ticketing.Application.Features.Tickets.Statuses;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
//[Authorize]
public sealed class TicketStatusesController : ControllerBase
{
    private readonly IMediator mediator;

    public TicketStatusesController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ItemsResult<TicketStatusDto>))]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesDefaultResponseType]
    public async Task<ItemsResult<TicketStatusDto>> GetStatuses(string? searchTerm, int page = 1, int pageSize = 10, string? sortBy = null, SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
        => await mediator.Send(new GetTicketStatuses(searchTerm, page, pageSize, sortBy, sortDirection), cancellationToken);

}
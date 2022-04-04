
using System.ComponentModel.DataAnnotations;

using YourBrand.Application.Notifications;
using YourBrand.Application.Notifications.Commands;
using YourBrand.Application.Notifications.Queries;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace YourBrand.WebApi.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class NotificationsController : Controller
{
    private readonly IMediator _mediator;

    public NotificationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<NotificationsResults>> GetNotifications(
        bool includeUnreadNotificationsCount = false,
        int page = 1, int pageSize = 5, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new GetNotificationsQuery(includeUnreadNotificationsCount, page, pageSize, sortBy, sortDirection), cancellationToken));
    }

    [HttpPost("{id}/MarkAsRead")]
    public async Task<ActionResult> MarkNotificationAsRead(string id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new MarkNotificationAsReadCommand(id), cancellationToken);

        return Ok();
    }

    [HttpPost("MarkAllAsRead")]
    public async Task<ActionResult> MarkAllNotificationsAsRead(CancellationToken cancellationToken)
    {
        await _mediator.Send(new MarkAllNotificationsAsReadCommand(), cancellationToken);

        return Ok();
    }
}
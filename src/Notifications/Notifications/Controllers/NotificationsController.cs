
using System.ComponentModel.DataAnnotations;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using YourBrand.Notifications.Application.Notifications;
using YourBrand.Notifications.Application.Notifications.Commands;
using YourBrand.Notifications.Application.Notifications.Queries;

namespace YourBrand.Notifications.WebApi.Controllers;

[Route("[controller]")]
[ApiController]
//[Authorize]
public class NotificationsController(IMediator mediator) : Controller
{
    [HttpGet]
    public async Task<ActionResult<NotificationsResults>> GetNotifications(
        string? userId, string? tag,
        bool includeUnreadNotificationsCount = false,
        int page = 1, int pageSize = 5, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new GetNotificationsQuery(userId, tag, includeUnreadNotificationsCount, page - 1, pageSize, sortBy, sortDirection), cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult> CreateNotification(CreateNotificationDto dto, CancellationToken cancellationToken)
    {
        await mediator.Send(new CreateNotificationCommand(dto.Content, dto.Link, dto.UserId, dto.ScheduledFor), cancellationToken);

        return Ok();
    }

    [HttpPost("{id}/MarkAsRead")]
    public async Task<ActionResult> MarkNotificationAsRead(string id, CancellationToken cancellationToken)
    {
        await mediator.Send(new MarkNotificationAsReadCommand(id), cancellationToken);

        return Ok();
    }

    [HttpPost("MarkAllAsRead")]
    public async Task<ActionResult> MarkAllNotificationsAsRead(CancellationToken cancellationToken)
    {
        await mediator.Send(new MarkAllNotificationsAsReadCommand(), cancellationToken);

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteNotification(string id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteNotificationCommand(id), cancellationToken);

        return Ok();
    }

    [HttpGet("UnreadCount")]
    public async Task<ActionResult<int>> GetUnreadNotificationsCount(string? userId, CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new GetUnreadNotificationsCountQuery(userId), cancellationToken));
    }
}

public class CreateNotificationDto
{
    [Required]
    public string Content { get; set; }

    public string? Link { get; set; }

    public string? UserId { get; set; }

    public DateTime? ScheduledFor { get; set; }
}
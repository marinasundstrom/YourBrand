
using YourBrand.Messenger.Application.Common.Interfaces;
using YourBrand.Messenger.Application.Common.Models;
using YourBrand.Messenger.Application.Conversations.Commands;
using YourBrand.Messenger.Application.Conversations.Queries;
using YourBrand.Messenger.Application.Messages.Commands;
using YourBrand.Messenger.Application.Messages.Queries;
using YourBrand.Messenger.Contracts;

using MassTransit;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YourBrand.Identity;

namespace YourBrand.Messenger.WebApi.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = Messenger.Authentication.AuthSchemes.Default)]
[Route("[controller]")]
public class ConversationsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IRequestClient<PostMessage> _postMessageClient;

    public ConversationsController(IMediator mediator, IRequestClient<PostMessage> postMessageClient)
    {
        _mediator = mediator;
        _postMessageClient = postMessageClient;
    }

    [HttpGet]
    public async Task<ActionResult<Results<ConversationDto>>> GetConversations(
        int skip = 0, int take = 10, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new GetConversationsQuery(skip, take, sortBy, sortDirection), cancellationToken));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ConversationDto>> GetConversations(
        string id,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new GetConversationQuery(id), cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateConversation(
        string? title, CancellationToken cancellationToken = default)
    {
        var dto = await _mediator.Send(new CreateConversationCommand(title), cancellationToken);
        return Ok(dto);
    }

    [HttpPost("{id}/Join")]
    public async Task<ActionResult> JoinConversation(
        string id, CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new JoinConversationCommand(id), cancellationToken);
        return Ok();
    }


    [HttpPost("{id}/Leave")]
    public async Task<ActionResult> LeaveConversation(
        string id, CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new LeaveConversationCommand(id), cancellationToken);
        return Ok();
    }

    [HttpGet("{id}/Messages")]
    public async Task<ActionResult<Results<MessageDto>>> GetMessages(
        string id, int skip = 0, int take = 10, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new GetMessagesIncrQuery(id, skip, take, sortBy, sortDirection), cancellationToken));
    }

    [HttpPost("{id}/Messages")]
    public async Task<ActionResult<MessageDto>> PostMessage(
        [FromServices] ICurrentUserService currentUserService,
        string id,
        string text, string? replyToId, CancellationToken cancellationToken = default)
    {
        var response = await _postMessageClient.GetResponse<MessageDto>(new PostMessage(currentUserService.GetAccessToken()!, id, text, replyToId), cancellationToken);
        return Ok(response.Message);
    }

    [HttpDelete("{conversationId}/Messages/{id}")]
    public async Task<ActionResult> DeleteMessage(
        string conversationId, string id,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new DeleteMessageCommand(id), cancellationToken);
        return Ok();
    }
}
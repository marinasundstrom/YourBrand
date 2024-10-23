using Asp.Versioning.Builder;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using YourBrand.ChatApp.Common;
using YourBrand.ChatApp.Domain;
using YourBrand.ChatApp.Domain.ValueObjects;
using YourBrand.Domain;
using YourBrand.Extensions;

namespace YourBrand.ChatApp.Features.Chat.Messages;

public static class Endpoints
{
    public static WebApplication MapMessageEndpoints(this WebApplication app)
    {
        var messages = app.NewVersionedApi("Messages");

        MapVersion1(messages);

        return app;
    }

    private static void MapVersion1(IVersionedEndpointRouteBuilder channels)
    {
        var group = channels.MapGroup("/v{version:apiVersion}/Channels/{channelId}/Messages")
            //.WithTags("Messages")
            .HasApiVersion(1, 0)
            .RequireAuthorization()
            .WithOpenApi();

        group.MapGet("/", GetMessages)
            .WithName($"Messages_{nameof(GetMessages)}")
            .Produces<ItemsResult<MessageDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status429TooManyRequests)
            .RequireRateLimiting("fixed");

        group.MapGet("/{id}", GetMessageById)
            .WithName($"Messages_{nameof(GetMessageById)}")
            .Produces<MessageDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithName(nameof(GetMessageById));

        group.MapPost("/", PostMessage)
            .WithName($"Messages_{nameof(PostMessage)}")
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status404NotFound);


        group.MapDelete("/{id}", DeleteMessage)
            .WithName($"Messages_{nameof(DeleteMessage)}")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPut("/{id}", EditMessage)
            .WithName($"Messages_{nameof(EditMessage)}")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/{id}/Reaction", React)
            .WithName($"Messages_{nameof(React)}")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/{id}/Reaction", RemoveReaction)
            .WithName($"Messages_{nameof(RemoveReaction)}")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    public static async Task<ItemsResult<MessageDto>> GetMessages(OrganizationId organizationId, ChannelId channelId, int page = 1, int pageSize = 10, string? sortBy = null, SortDirection? sortDirection = null, CancellationToken cancellationToken = default, IMediator mediator = default!)
        => await mediator.Send(new GetMessages(organizationId, channelId, page, pageSize, sortBy, sortDirection), cancellationToken);


    public static async Task<IResult> GetMessageById(OrganizationId organizationId, ChannelId channelId, MessageId id, CancellationToken cancellationToken, IMediator mediator)
    {
        var result = await mediator.Send(new GetMessageById(organizationId, channelId, id), cancellationToken);
        return HandleResult(result);
    }

    public static async Task<IResult> PostMessage(OrganizationId organizationId, ChannelId channelId, PostMessageRequest request, CancellationToken cancellationToken, IMediator mediator)
    {
        var result = await mediator.Send(new PostMessage(organizationId, request.ChannelId, request.ReplyToId, request.Content), cancellationToken);
        return result.Handle(
            onSuccess: data => Results.CreatedAtRoute(nameof(GetMessageById), new { id = data }, data),
            onError: error => Results.Problem(detail: error.Detail, title: error.Title, type: error.Id));
    }

    public static async Task<IResult> DeleteMessage(OrganizationId organizationId, ChannelId channelId, MessageId id, CancellationToken cancellationToken, IMediator mediator)
    {
        var result = await mediator.Send(new DeleteMessage(organizationId, channelId, id), cancellationToken);
        return HandleResult(result);
    }

    public static async Task<IResult> EditMessage(OrganizationId organizationId, ChannelId channelId, MessageId id, [FromBody] string content, CancellationToken cancellationToken, IMediator mediator)
    {
        var result = await mediator.Send(new EditMessage(organizationId, channelId, id, content), cancellationToken);
        return HandleResult(result);
    }

    public static async Task<IResult> React(OrganizationId organizationId, ChannelId channelId, MessageId id, [FromBody] string reaction, CancellationToken cancellationToken, IMediator mediator)
    {
        var result = await mediator.Send(new React(organizationId, channelId, id, reaction), cancellationToken);
        return HandleResult(result);
    }

    public static async Task<IResult> RemoveReaction(OrganizationId organizationId, ChannelId channelId, MessageId id, [FromBody] string reaction, CancellationToken cancellationToken, IMediator mediator)
    {
        var result = await mediator.Send(new RemoveReaction(organizationId, channelId, id, reaction), cancellationToken);
        return HandleResult(result);
    }

    /*
     public static async Task<IResult> UpdateDescription(int id, [FromBody] string? description, CancellationToken cancellationToken, IMediator mediator)
     {
         var result = await mediator.Send(new UpdateDescription(id, description), cancellationToken);
         return HandleResult(result);
     }

     public static async Task<IResult> UpdateStatus(int id, [FromBody] MessageStatusDto status, CancellationToken cancellationToken, IMediator mediator)
     {
         var result = await mediator.Send(new UpdateStatus(id, status), cancellationToken);
         return HandleResult(result);
     }

     public static async Task<IResult> UpdateAssignedUser(int id, [FromBody] string? userId, CancellationToken cancellationToken, IMediator mediator)
     {
         var result = await mediator.Send(new UpdateAssignedUser(id, userId), cancellationToken);
         return HandleResult(result);
     }

     public static async Task<IResult> UpdateEstimatedHours(int id, [FromBody] double? hours, CancellationToken cancellationToken, IMediator mediator)
     {
         var result = await mediator.Send(new UpdateEstimatedHours(id, hours), cancellationToken);
         return HandleResult(result);
     }

     public static async Task<IResult> UpdateRemainingHours(int id, [FromBody] double? hours, CancellationToken cancellationToken, IMediator mediator)
     {
         var result = await mediator.Send(new UpdateRemainingHours(id, hours), cancellationToken);
         return HandleResult(result);
     }
             */

    private static IResult HandleResult(Result result) => result.Handle(
            onSuccess: () => Results.Ok(),
            onError: error =>
            {
                if (error.Id.EndsWith("NotFound"))
                {
                    return Results.NotFound();
                }
                return Results.Problem(detail: error.Detail, title: error.Title, type: error.Id);
            });

    private static IResult HandleResult<T>(Result<T> result) => result.Handle(
            onSuccess: data => Results.Ok(data),
            onError: error =>
            {
                if (error.Id.EndsWith("NotFound"))
                {
                    return Results.NotFound();
                }
                return Results.Problem(detail: error.Detail, title: error.Title, type: error.Id);
            });
}
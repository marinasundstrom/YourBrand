using YourCompany.Application;
using YourCompany.Application.Common.Models;
using YourCompany.Application.Items;
using YourCompany.Application.Items.Commands;
using YourCompany.Application.Items.Queries;
using YourCompany.Infrastructure.Persistence;
using YourCompany.WebApi.Controllers;

using MediatR;

using Microsoft.Extensions.Caching.Distributed;

using MiniValidation;

namespace YourCompany.WebApi;

static partial class RequestHandlers
{
    public static WebApplication MapApplicationRequests(this WebApplication app)
    {
        app.MapGet("/", GetItems)
        .WithName("Items_GetItems")
        .WithTags("Items")
        .RequireAuthorization()
        .Produces<Results<ItemDto>>(StatusCodes.Status200OK);

        app.MapGet("/{id}", GetItem)
        .WithName("Items_GetItem")
        .WithTags("Items")
        .RequireAuthorization()
        .Produces<ItemDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        app.MapPut("/{id}", UpdateItem)
        .WithName("Items_UpdateItem")
        .WithTags("Items")
        .RequireAuthorization()
        .Produces(StatusCodes.Status200OK);

        app.MapPost("/", AddItem)
        .WithName("Items_AddItem")
        .WithTags("Items")
        .RequireAuthorization()
        .Produces(StatusCodes.Status200OK)
        .ProducesValidationProblem(StatusCodes.Status400BadRequest);

        app.MapDelete("/{id}", DeleteItem)
        .WithName("Items_DeleteItem")
        .WithTags("Items")
        .RequireAuthorization()
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        app.MapGet("/{id}/Comments", GetComments)
        .WithName("Items_GetComments")
        .WithTags("Items")
        .RequireAuthorization()
        .Produces<Results<CommentDto>>(StatusCodes.Status200OK);

        app.MapGet("/{id}/Comments/{commentId}", GetComment)
       .WithName("Items_GetComment")
       .WithTags("Items")
       .RequireAuthorization()
       .Produces<CommentDto>(StatusCodes.Status200OK)
       .Produces(StatusCodes.Status404NotFound);

        app.MapPost("/{id}/Comments", PostComment)
        .WithName("Items_PostComment")
        .WithTags("Items")
        .RequireAuthorization()
        .Produces(StatusCodes.Status200OK);

        app.MapPut("/{id}/Comments/{commentId}", UpdateComment)
        .WithName("Items_UpdateComment")
        .WithTags("Items")
        .RequireAuthorization()
        .Produces(StatusCodes.Status200OK);

        app.MapDelete("/{id}/Comments/{commentId}", DeleteComment)
        .WithName("Items_DeleteComment")
        .WithTags("Items")
        .RequireAuthorization()
        .Produces(StatusCodes.Status200OK);

        return app;
    }

    static async Task<IResult> GetItems(IMediator mediator, CancellationToken cancellationToken, int page = 0, int pageSize = 10,
        string? sortBy = null, Application.Common.Models.SortDirection sortDirection = Application.Common.Models.SortDirection.Desc)
    {
        var result = await mediator.Send(new GetItemsQuery(page, pageSize, sortBy, sortDirection), cancellationToken);

        return Results.Ok(result);
    }

    static async Task<IResult> GetItem(string id, IMediator mediator, IDistributedCache cache, CancellationToken cancellationToken)
    {
        string cacheKey = $"item-{id}";

        var item = await cache.GetAsync<ItemDto?>(cacheKey, cancellationToken);

        if (item is null)
        {
            item = await mediator.Send(new GetItemQuery(id), cancellationToken);

            if (item == null)
            {
                return Results.NotFound();
            }

            await cache.SetAsync(cacheKey, item,
                new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1) },
                cancellationToken);
        }

        return Results.Ok(item);
    }

    static async Task<IResult> UpdateItem(string id, UpdateItemDto dto, IMediator mediator, IDistributedCache cache, CancellationToken cancellationToken)
    {
        if (!MiniValidator.TryValidate(dto, out var errors))
        {
            return Results.ValidationProblem(errors, statusCode: StatusCodes.Status400BadRequest);
        }

        string cacheKey = $"item-{id}";

        await cache.RemoveAsync(cacheKey);

        await mediator.Send(new UpdateItemCommand(id, dto.Name, dto.Description), cancellationToken);

        return Results.Ok();
    }

    static async Task<IResult> AddItem(AddItemDto dto, IMediator mediator, CancellationToken cancellationToken)
    {
        if (!MiniValidator.TryValidate(dto, out var errors))
        {
            return Results.ValidationProblem(errors, statusCode: StatusCodes.Status400BadRequest);
        }

        await mediator.Send(new AddItemCommand(dto.Name, dto.Description), cancellationToken);

        return Results.Ok();
    }

    static async Task<IResult> DeleteItem(string id, IMediator mediator, IDistributedCache cache, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteItemCommand(id), cancellationToken);

        if (result == DeletionResult.NotFound)
        {
            return Results.NotFound();
        }

        string cacheKey = $"item-{id}";

        await cache.RemoveAsync(cacheKey, cancellationToken);

        return Results.Ok();
    }

    static async Task<IResult> GetComments(IMediator mediator, CancellationToken cancellationToken, string id, int page = 0, int pageSize = 10,
        string? sortBy = null, Application.Common.Models.SortDirection sortDirection = Application.Common.Models.SortDirection.Desc)
    {
        var result = await mediator.Send(new GetCommentsQuery(id, page, pageSize, sortBy, sortDirection), cancellationToken);

        return Results.Ok(result);
    }

    static async Task<IResult> GetComment(string id, string commentId, IMediator mediator, IDistributedCache cache, CancellationToken cancellationToken)
    {
        string cacheKey = $"comment-{commentId}";

        var item = await cache.GetAsync<CommentDto?>(cacheKey, cancellationToken);

        if (item is null)
        {
            item = await mediator.Send(new GetCommentQuery(commentId), cancellationToken);

            if (item == null)
            {
                return Results.NotFound();
            }

            await cache.SetAsync(cacheKey, item,
                new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1) },
                cancellationToken);
        }

        return Results.Ok(item);
    }

    static async Task<IResult> PostComment(string id, PostCommentDto dto, IMediator mediator, CancellationToken cancellationToken)
    {
        if (!MiniValidator.TryValidate(dto, out var errors))
        {
            return Results.ValidationProblem(errors, statusCode: StatusCodes.Status400BadRequest);
        }

        await mediator.Send(new PostCommentCommand(id, dto.Text), cancellationToken);

        return Results.Ok();
    }

    static async Task<IResult> UpdateComment(string id, string commentId, UpdateCommentDto dto, IMediator mediator, IDistributedCache cache, CancellationToken cancellationToken)
    {
        if (!MiniValidator.TryValidate(dto, out var errors))
        {
            return Results.ValidationProblem(errors, statusCode: StatusCodes.Status400BadRequest);
        }

        string cacheKey = $"comment-{commentId}";

        await cache.RemoveAsync(cacheKey);

        await mediator.Send(new UpdateCommentCommand(commentId, dto.Text), cancellationToken);

        return Results.Ok();
    }

    static async Task<IResult> DeleteComment(string id, string commentId, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteCommentCommand(commentId), cancellationToken);

        return Results.Ok();
    }
}
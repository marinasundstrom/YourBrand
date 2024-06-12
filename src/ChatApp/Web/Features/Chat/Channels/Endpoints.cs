using Asp.Versioning.Builder;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ChatApp.Common;
using ChatApp.Domain;
using ChatApp.Extensions;

namespace ChatApp.Features.Chat.Channels;

public static class Endpoints
{
    public static WebApplication MapChannelEndpoints(this WebApplication app)
    {
        var channels = app.NewVersionedApi("Channels");

        MapVersion1(channels);

        return app;
    }

    private static void MapVersion1(IVersionedEndpointRouteBuilder channels)
    {
        var group = channels.MapGroup("/v{version:apiVersion}/Channels")
            //.WithTags("Channels")
            .HasApiVersion(1, 0)
            .RequireAuthorization()
            .WithOpenApi();

          
        group.MapGet("/", GetChannels)
            .WithName($"Channels_{nameof(GetChannels)}")
            .Produces<ItemsResult<ChannelDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status429TooManyRequests)
            .RequireRateLimiting("fixed");

        group.MapGet("/{id}", GetChannelById)
            .WithName($"Channels_{nameof(GetChannelById)}")
            .Produces<ChannelDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
            
        group.MapPost("/", CreateChannel)
            .WithName($"Channels_{nameof(CreateChannel)}")
            .Produces<ChannelDto>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    public static async Task<ItemsResult<ChannelDto>> GetChannels(int page = 1, int pageSize = 10, string? sortBy = null, SortDirection? sortDirection = null, CancellationToken cancellationToken = default, IMediator mediator = default!)
        => await mediator.Send(new GetChannels(page, pageSize, sortBy, sortDirection), cancellationToken);

    public static async Task<IResult> GetChannelById(Guid id, CancellationToken cancellationToken, IMediator mediator)
    {
        var result = await mediator.Send(new GetChannelById(id), cancellationToken);
        return HandleResult(result);
    }

    public static async Task<IResult> CreateChannel(CreateChannelRequest request, CancellationToken cancellationToken, IMediator mediator)
    {
        var result = await mediator.Send(new CreateChannel(request.Name), cancellationToken);
        return result.Handle(
            onSuccess: data => Results.CreatedAtRoute($"Channels_{nameof(GetChannelById)}", new { id = data.Id }, data),
            onError: error => Results.Problem(detail: error.Detail, title: error.Title, type: error.Id));
    }

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

public sealed record CreateChannelRequest(string Name);
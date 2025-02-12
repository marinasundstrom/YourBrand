﻿using Asp.Versioning.Builder;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using YourBrand.ChatApp.Common;
using YourBrand.ChatApp.Domain;
using YourBrand.ChatApp.Domain.ValueObjects;
using YourBrand.Domain;
using YourBrand.Extensions;

namespace YourBrand.ChatApp.Features.Chat.Channels;

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

        group.MapPost("/{id}/join", JoinChannel)
        .WithName($"Channels_{nameof(JoinChannel)}")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }

    public static async Task<ItemsResult<ChannelDto>> GetChannels(OrganizationId organizationId, int page = 1, int pageSize = 10, string? sortBy = null, SortDirection? sortDirection = null, CancellationToken cancellationToken = default, IMediator mediator = default!)
        => await mediator.Send(new GetChannels(organizationId, page, pageSize, sortBy, sortDirection), cancellationToken);

    public static async Task<IResult> GetChannelById(OrganizationId organizationId, ChannelId id, CancellationToken cancellationToken, IMediator mediator)
    {
        var result = await mediator.Send(new GetChannelById(organizationId, id), cancellationToken);
        return HandleResult(result);
    }

    public static async Task<IResult> CreateChannel(OrganizationId organizationId, CreateChannelRequest request, CancellationToken cancellationToken, IMediator mediator)
    {
        var result = await mediator.Send(new CreateChannel(organizationId, request.Name), cancellationToken);
        return result.Handle(
            onSuccess: data => Results.CreatedAtRoute($"Channels_{nameof(GetChannelById)}", new { id = data.Id }, data),
            onError: error => Results.Problem(detail: error.Detail, title: error.Title, type: error.Id));
    }

    public static async Task<IResult> JoinChannel(OrganizationId organizationId, ChannelId id, CancellationToken cancellationToken, IMediator mediator)
    {
        var result = await mediator.Send(new JoinChannel(organizationId, id), cancellationToken);
        return result.Handle(
            onSuccess: () => Results.Ok(),
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
using MediatR;

namespace YourBrand.StoreFront.API.Features.Checkout;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapCheckoutEndpoints(this IEndpointRouteBuilder app)
    {
        string GetCartsExpire20 = nameof(GetCartsExpire20);

        var versionedApi = app.NewVersionedApi("Checkout");

        var cartGroup = versionedApi.MapGroup("/v{version:apiVersion}")
            .WithTags("Checkout")
            .HasApiVersion(ApiVersions.V1)
            .WithOpenApi()
            .RequireCors();

        cartGroup.MapPost("/checkout", Checkout)
            .WithName($"Checkout_{nameof(Checkout)}");

        return app;
    }

    public static async Task Checkout(CheckoutDto dto, IMediator mediator, CancellationToken cancellationToken = default)
    {
        await mediator.Send(new Checkout(dto.BillingDetails, dto.ShippingDetails), cancellationToken);
    }
}

public class CheckoutDto
{
    public BillingDetailsDto BillingDetails { get; set; } = null!;

    public ShippingDetailsDto ShippingDetails { get; set; } = null!;
}
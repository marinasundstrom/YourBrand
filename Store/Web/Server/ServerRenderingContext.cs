namespace BlazorApp;

public sealed class ServerRenderingContext : RenderingContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ServerRenderingContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override bool IsServer => true;

    public override bool IsClient => false;

    public override bool IsPrerendering => !_httpContextAccessor.HttpContext!.Response.HasStarted;
}
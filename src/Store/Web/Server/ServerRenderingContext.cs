namespace BlazorApp;

public sealed class ServerRenderingContext(IHttpContextAccessor httpContextAccessor) : RenderingContext
{
    public override bool IsServer => true;

    public override bool IsClient => false;

    public override bool IsPrerendering => !httpContextAccessor.HttpContext!.Response.HasStarted;
}
namespace BlazorApp;

public sealed class ClientRenderingContext : RenderingContext
{
    public override bool IsServer => false;

    public override bool IsClient => true;

    public override bool IsPrerendering => false;
}
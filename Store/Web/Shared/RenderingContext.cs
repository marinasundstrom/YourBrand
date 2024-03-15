namespace BlazorApp;

public abstract class RenderingContext
{
    public virtual bool IsServer { get; }

    public virtual bool IsClient { get; }

    public virtual bool IsPrerendering { get; }
}
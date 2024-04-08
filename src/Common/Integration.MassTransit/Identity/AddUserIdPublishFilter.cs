using MassTransit;

using YourBrand.Identity;

namespace YourBrand.Integration;

public sealed class AddUserIdPublishFilter<T>(IUserContext userContext) :
    IFilter<PublishContext<T>>
    where T : class
{
    public void Probe(ProbeContext context)
    {
    }

    public Task Send(PublishContext<T> context, IPipe<PublishContext<T>> next)
    {
        context.Headers.Set(Constants.UserId, userContext.UserId);

        return next.Send(context);
    }
}
using MassTransit;

using YourBrand.Identity;

namespace YourBrand.Integration;

public class ReadUserIdConsumeFilter<T>(ISettableUserContext userContext) :
    IFilter<ConsumeContext<T>>
    where T : class
{
    public void Probe(ProbeContext context)
    {
    }

    public Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        var userId = context.Headers.Get<string>(Constants.UserId);

        userContext.SetCurrentUser(userId!);

        return next.Send(context);
    }
}
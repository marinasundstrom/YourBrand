using MassTransit;

using YourBrand.Identity;

namespace YourBrand.Integration;

public class AddUserIdSendFilter<T>(IUserContext userContext) :
    IFilter<SendContext<T>>
    where T : class
{
    public void Probe(ProbeContext context)
    {
    }

    public Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
    {
        context.Headers.Set(Constants.UserId, userContext.UserId);

        return next.Send(context);
    }
}
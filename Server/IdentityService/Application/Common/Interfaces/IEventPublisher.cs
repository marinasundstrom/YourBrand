namespace Skynet.IdentityService.Application.Common.Interfaces;

public interface IEventPublisher
{
    Task PublishEvent(object ev);
}
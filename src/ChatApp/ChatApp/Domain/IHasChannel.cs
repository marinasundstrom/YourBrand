namespace YourBrand.ChatApp.Domain;

using YourBrand.ChatApp.Domain.ValueObjects;

public interface  IHasChannel
{
    ChannelId ChannelId { get; }
}
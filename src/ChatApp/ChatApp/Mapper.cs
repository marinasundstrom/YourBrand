﻿using YourBrand.ChatApp.Features.Chat;
using YourBrand.ChatApp.Features.Chat.Channels;
using YourBrand.ChatApp.Features.Organizations;
using YourBrand.ChatApp.Features.Users;

namespace YourBrand.ChatApp;

public static class Mappings
{
    public static ChannelDto ToDto(this Channel channel) => new(channel.Id, channel.Title);

    public static UserDto ToDto(this User user) => new(user.Id, user.Name);

    public static UserInfoDto ToDto2(this User user) => new(user.Id, user.Name);

    public static OrganizationDto ToDto(this Organization user) => new(user.Id, user.Name);

    public static OrganizationDto ToDto2(this Organization user) => new(user.Id, user.Name);
}
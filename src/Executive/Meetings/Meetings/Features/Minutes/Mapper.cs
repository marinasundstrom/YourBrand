﻿using YourBrand.Meetings.Features.Agendas;
using YourBrand.Meetings.Features.Organizations;
using YourBrand.Meetings.Features.Users;

namespace YourBrand.Meetings.Features.Minutes;

public static partial class Mappings
{
    public static MinutesDto ToDto(this Domain.Entities.Minutes minute) => new(minute.Id, minute.MeetingId, minute.State, minute.Items.Select(x => x.ToDto()));
    public static MinutesItemDto ToDto(this MinutesItem item) => new(item.Id, item.Order, item.Type.ToDto(), item.AgendaId, item.AgendaItemId, item.Heading!, item.State, item.Details, item.MotionId);
}
using YourBrand.Meetings.Features.Users;
using YourBrand.Meetings.Features.Organizations;

namespace YourBrand.Meetings.Features.Agendas;

public static partial class Mappings
{
    public static AgendaDto ToDto(this Agenda agenda) => new(agenda.Id, agenda.State, agenda.Items.Select(x => x.ToDto()));
    public static AgendaItemDto ToDto(this AgendaItem item) => new(item.Id, item.Order , item.Type, item.Title!, item.State, item.Description, item.MotionId);
}
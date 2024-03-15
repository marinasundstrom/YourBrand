using YourBrand.Application.Widgets;
using YourBrand.Domain.Entities;

namespace YourBrand.Application;

public static class Mapper
{
    public static WidgetDto ToDto(this Widget widget) => new(widget.Id, widget.WidgetId, widget.WidgetAreaId, widget.UserId, widget?.Settings?.RootElement.ToString());
}
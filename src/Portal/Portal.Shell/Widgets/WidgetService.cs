using System;
using System.Data;
using System.Security.Claims;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

using YourBrand.Portal.Services;

namespace YourBrand.Portal.Widgets;


public interface IWidgetService
{
    IReadOnlyCollection<Widget> Widgets { get; }

    void RegisterWidget(Widget widget);

    void RemoveWidget(string id);

    event EventHandler? WidgetAdded;

    event EventHandler? WidgetRemoved;

    event EventHandler? WidgetUpdated;
}

public sealed class Widget
{
    private string? name;
    private Func<string>? nameFunc;
    private bool isVisible = true;
    private WidgetSize size = WidgetSize.Small;
    private bool isHeaderVisible = true;

    public Widget(string id, string name, Type componentType)
    {
        Id = id;
        Name = name;
        ComponentType = componentType;
    }

    public Widget(string id, Func<string> nameFunc, Type componentType)
    {
        Id = id;
        NameFunc = nameFunc;
        ComponentType = componentType;
    }

    public string Id { get; }

    public string Name
    {
        get => name ?? nameFunc?.Invoke() ?? throw new Exception();
        set
        {
            if (value != name)
            {
                name = value;
                Updated?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public Func<string>? NameFunc
    {
        get => nameFunc;
        set
        {
            if (value != nameFunc)
            {
                name = null;
                nameFunc = value;
                Updated?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public WidgetSize Size
    {
        get => size;
        set
        {
            if (value != size)
            {
                size = value;
                Updated?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public Type? ComponentType { get; }

    public bool IsVisible
    {
        get => isVisible;
        set
        {
            if (value != isVisible)
            {
                isVisible = value;
                Updated?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public void Hide() => IsVisible = false;

    public void Show() => IsVisible = true;

    public bool IsHeaderVisible
    {
        get => isHeaderVisible;
        set
        {
            if (value != isHeaderVisible)
            {
                isHeaderVisible = value;
                Updated?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public bool RequiresAuthorization { get; set; }

    public IEnumerable<string>? Roles { get; set; }

    public string? Policy { get; set; }

    public event EventHandler? Updated;
}

public enum WidgetSize
{
    Small,
    Medium,
    Large
}

public sealed class WidgetService : IWidgetService, IDisposable
{
    private readonly IDictionary<string, Widget> _widgets = new Dictionary<string, Widget>();

    public IReadOnlyCollection<Widget> Widgets => _widgets.Select(x => x.Value).ToList();

    public event EventHandler? WidgetAdded;

    public event EventHandler? WidgetRemoved;

    public event EventHandler? WidgetUpdated;

    void IWidgetService.RegisterWidget(Widget widget)
    {
        _widgets.Add(widget.Id, widget);
        WidgetAdded?.Invoke(this, EventArgs.Empty);

        widget.Updated += OnWidgetUpdated;
    }

    void IWidgetService.RemoveWidget(string id)
    {
        var widget = _widgets[id];

        _widgets.Remove(id);
        WidgetRemoved?.Invoke(this, EventArgs.Empty);

        widget.Updated -= OnWidgetUpdated;
    }

    private void OnWidgetUpdated(object? sender, EventArgs e)
    {
        WidgetUpdated?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose()
    {
        foreach (var widget in Widgets)
        {
            widget.Updated -= OnWidgetUpdated;
        }
    }
}
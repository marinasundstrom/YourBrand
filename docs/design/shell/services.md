# Services

The shell provides the following services:

## Navigation

Main navigation is done through the Nav Menu.

The nav menu consists of groups and items. Items may have sub-items.

The menu is managed by the NavManager.

## AppBarTray

The app bar tray is an area of AppBar where buttons can be placed.

The shell provides some out-of-box: Localization and Theme selectors.

## Theming

There are two themes: Light and Dark mode. 

And there is a mode with which the theme changes depending on the system settings.

ThemeManager is responsible for the theme.

## Widgets

Widgets are components that can be placed on a page.

A Widget is rendered in a named WidgetArea.

The WidgetService takes care of Widgets.

## Localization

Supported at the moment: English (default), and Swedish.

## Search

There is a search box in the AppBar.

The functionality has yet to be implemented.

## Notifications

Presents notifications for users.

## UI Modules

Each service may have a module that provides UI for that service - like pages and other services.

This is analogous to a plug-in.

A UI module is initialized through a "module initializer" in which the service above can be retrieved and invoked, in order to add items to the menu, to the app bar tray, or to register widgets.
# Web API

The Web API project is a web service that exposes the server-side application.

The main application logic is in a separate project, called Application.

The standard mode of accessing the service is via HTTP API. It also has WebSocket support, using SignalR.

The service communicates back and forth with the Worker by sending asynchronous messages on the message bus.

## Authorization

Some parts of the Web API requires the user to be authorized.

These parts are: Items, Notifications, and Search.

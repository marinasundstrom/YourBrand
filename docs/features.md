# Features

This document explains the features of this solution.

The logic is implemented in the projects called Application and Worker.

## Items

The user can view, add, and deleted items. As an option the user can upload a picture, as well. 

Changes to the items list will instantly reflect in all clients thanks to notifications being sent from the server.

## Notifications

The app may receive notifications that are published by the Worker.

The Worker manages notifications and publishes them to either all users or a specific user. Also does scheduled notifications.

## Async operation

The user sends two numbers and get a response back.

The client request is HTTP API, and the response is sent back via WebSocket.

The request arrives at the Web API that emits a message on the message bus. The Worker consumes that message, performs an operation, to then publish a response message. 

The Web API consumes the response message, and emits it to the Web Socket, using SignalR. The client receives and displays the response.

## Worker (Ping)

The Worker runs a recurring task every minute that emits a message that is handled similar to the feature above. 

The message is published so that anybody who wants can receive it.

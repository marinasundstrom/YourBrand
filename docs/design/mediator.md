# Mediator pattern

At the core of the Mediator pattern is the mediator which handles interaction between a set of objects. In that way, decoupling them.

It can be implemented in several ways, but in our case: 

It can be seen as passing a message from a sender to a mediator. The mediator then hands the message to the receiver, based on some rule, or attribute. The sender does not know about the receiver. In that way we achieve the desired loose coupling.

Mediator implementations, like MediatR, could be a messaging library that distinguish between Requests and Notifications. Requests may only have one receiver. Think of a command. Notifications can have multiple handlers that listen to a specific kind of notification. But at its core, there is an object in the middles.

In Async Messaging, a message broker is such a mediator.

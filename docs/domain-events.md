# Domain Events

The AppService is event-driven. It uses events as a mean for one part of the app to publish notifications that notify other parts that something has happened. Parts then subscribe to those events and act accordingly.

Domain events are separate from "Integration events". Integration events are available between processes using async messaging. Example of that is the notifications event that the Worker publishes
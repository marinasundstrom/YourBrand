# Asynchronous messaging

The Web API communicates with the Worker by sending messages using asynchronous messaging, facilitated by the MassTransit framework, with RabbitMQ as transport.

Messages get handled by RabbitMQ, a message-broker software. Each message is sent to an exchange and then delivered to one or many queues. A consumer subscribes to a queue that acts as a postbox to pull messages from. 

Since messaging is handled out-of-process, it does not affect the performance of any of the consuming services.

## Implementation

In the WebApi and Worker projects respectively, the Consumers folders house the consumers for the asynchronous messaging between the services. Think of these as the message or event handlers. 

All the Message types are in the Contracts project.

Remember hat messages can be received by multiple consumers. There is also an option to act as RPC calls. This project is not currently using that as such.

In the Worker project, there is a Notifier service that wraps the MassTransit stuff that publishes an event when the recurring task executes (every minute).  

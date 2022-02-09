# Command Query Response Segregation (CQRS)

The Command Query Response Segregation (CQRS) pattern, tells us that we should separate how we read or query data from when we create or modify the data in our code.

In this solution this pattern is applied in the form of having Commands (Writes) and Queries (Read), and the Mediator pattern on top of it. 

A Query or a Command has its own request type and request handler. Hence, we follow the CQRS pattern.
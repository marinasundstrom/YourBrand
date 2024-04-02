# Guidelines

## Evolving a Monolith

Start with a minimal structure in a single project that (initially) will contain the entire application.

This is a "Monolith".

Parts that may be shared are Domain, Data Access, and other common code. Apply abstractions and interfaces when appropriate.

Do not primarily group stuff in you application by technical concern, like folders for Controllers, RequestHandlers, Services etc. Instead group by feature, like TaxReturns.

However, Bounded Contexts, or Features, should stay separate and not be directly dependent on each other.

That makes the application a "Distributed Monolith".

Don't add any unnecessary complexity just because it would be "nice to have".

The key to maintaining the structure of a monolithic code base is continuous refactoring. When something get less maintainable then you move it to a place (a folder or a namespace) where it makes more sense. You group stuff together into cohesive units so that they are easily discovered. Remove dead code when you encounter it.

When the bounded contexts or features get too big, consider extracting them into their own independent projects. 

It will still be a Monolith because the code projects all live in the same repository and gets released together with other projects as a complete application.

### Evolving a Monolith into Microservices

Should you decide to split the application into microservices then that would be rather trivial.

You need to turn every module into an independent microservice application by creating separate hosting projects containing what is needed. Depending on whether the bounded contexts of the original Monolith shared a database you will have to split the database somehow. This will imply splitting the Data Access Layer in code, as well.
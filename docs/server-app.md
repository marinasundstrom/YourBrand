# Server App

This document explains the structure and tech of the server app - aka the Web Service.

To learn more about the features, please look at the Features document.

Details about the Web Api (The Web App itself) can be found in WebApi document.

## Layers

The server app consists of layers that each correspond to a project:

* Application - The application/business layer -
* Domain - Domain objects and logic
* Infrastructure - Handles persistence to a database and other basic services.
* Web Api - The Presentation layer - Exposes the app as a Web app with a Web API.

## Application API

The Application layer implements operations that the Presentation layer, i.e. the Web API, consumes. This is the default API.

Operations are exposed as Requests - Commands and Queries, following the CQRS pattern with Mediator. The actual implementation exists in Request Handlers.

Examples:

* GetItemsQuery
* AddItemCommand

## Data

A database is mapped to entities by Entity Framework.

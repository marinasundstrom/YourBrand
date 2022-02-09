# Item Catalog

App for cataloging items with pictures.

Built with .NET MAUI using Blazor. Components are from MudBlazor. It is multi-platform.

There is also a Web version of the app.

A "Worker" service demonstrates how to communicate with a microservice through asynchronous messaging.

**For more info:**

* Watch [demo video 1](https://youtu.be/wXaQB18FvRk) and [video 2](https://youtu.be/DGNxoAn2ywU).

* Features explained [here](https://github.com/marinasundstrom/item-catalog/blob/main/docs/features.md).

* Read the [documentation](https://github.com/marinasundstrom/item-catalog/tree/main/docs).

## Introduction

Multi-platform app, with Web backend and API. Written entirely in .NET.

App can be compiled to run on iOS, macOS, Android, Windows, and, of course, Web. Thanks to .NET and Blazor.

<a href="/docs/overview.PNG">
<img src="/docs/overview.PNG" height="450"  alt="Overview"  /></a>

Contains a WebApi. Uses SQL Server and emulated Azure Blob Storage out of box.

The "Worker" service is used to demonstrate how to off-load tasks to a micro service using asynchronous messaging. The library used is MassTransit, and for transport it uses RabbitMQ. It also Hangfire to schedule recurring tasks.

SignalR is used for real-time communication between server and client.

Duende Identity Server is used for handling user identities and authentication.

Nginx is used as a reverse-proxy.

Dev environment is based around projects and containers orchestrated by .NET Tye. Enabling you to launch all backend services with one simple command.

Redis is also integrated.

## Screenshots

<!-- <a href="/Screenshots/macOS.png">
<img src="/Screenshots/macOS.png" height="250"  alt="macOS" /></a>

<a href="/Screenshots/iPhone.png">
<img src="/Screenshots/iPhone.png" height="250"  alt="iPhone"  /></a> -->

<a href="/Screenshots/web.png">
<img src="/Screenshots/web.png" height="250"  alt="Web"  /></a>

Web App

## Getting started

Read this [document](https://github.com/marinasundstrom/item-catalog/tree/main/docs/getting-started.md).

## MAUI, what?

.NET Multi-platform App UI (MAUI) is an modern open-source app framework that is the evolution of Xamarin.Forms. Enabling you to build native apps with .NET - not just mobile but also desktop. It still has the XAML model for declaring UI.

But in this app, it is combined with Blazor.

MAUI contains abstractions that make it easier to leverage common functionality across the platforms. Camera, File Pickers, Compass etc.

## Blazor

Blazor is an open-source framework for building apps for the Web using components, based on the familiar Razor syntax.

It lets you combine HTML, CSS and C# to build interactive experiences for the Web.

There are two kinds of Blazor Web apps: Client-side (WebAssembly) and Server-side rendered.

But, in this MAUI app it works a bit different.
### Hybrid Native app

Despite the UI being built with HTML and CSS, there is no Web Server or Web Assembly in this app. The UI is rendered in the same process as the rest of the app. :) 

With some work, you can share components with your standard Blazor Web App.

This approach is similar to *React Native*, which lets you use React components that have been written for the web with mobile apps. The obvious difference is that, instead of React components and JavaScript,  Blazor lets your use Razor components and C# across the stack.

## ASP.NET Core

ASP.NET Core is an open-source framework for building Web Apps. 

You can build apps with MVC or Razor pages. You can also build Web APIs. Now we have true "Minimal" APIs which are hooking directly into the core

You can mix and match the technologies since they are based on the same core. Essentially, a pipeline of middlewares that handle HTTP requests.

Blazor (Server-side) is actually a part of ASP.NET Core too - so everything works great together. 

There are many libraries and third-party frameworks that can enhance your ASP.NET Core apps. Open API and Swagger UI is supported out of box via third-party library.

In this app, the server-side logic is hosted in an ASP.NET Core app that exposes a Web API. 

Using Open API you can generate client classes that are used by the MAUI app. Essentially, it hides away the details of making requests to the server via HTTP.

## Tye

A challenge when building distributed applications is to orchestrate services during development time. It might be your own projects, database server, Nginx. How to configure them, and make them communicate.

You can always run stuff locally or in containers, but that implies manual configuration. Setting up a virtual network. Tye handles all this for you. 

Much like full orchestrators, such as *Docker Compose*, it gives you a way to declare your services and the desired configuration in a file (```tye.yaml```). It then runs your projects and containers (in Docker), setting up networks and such for you. Even enabling service discovery.

You can check in this ```tye.yaml``` file with your source code to let other developers get going in no time.

When you are ready to release your application, Tye helps you publish it to Kubernetes.

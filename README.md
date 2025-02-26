# YourBrand

Enterprise system for e-commerce and consulting services (Prototype).

Modular solution with Portal and e-Shop apps.

Watch this [video](https://www.youtube.com/watch?v=k6SftNnsILo) demoing the app.

[Video](https://www.youtube.com/watch?v=rQgaeXCR9eY) of the  Store (2025)

## Getting started

Read this [document](/docs/development/getting-started.md).

## Background

The seed of this project was planted in 2018, with ["Showroom"](https://github.com/marinasundstrom/YourBrand) - a site for presenting consultants.

In the summer of 2021, I experimented with building a ["Point of Sale" system](https://github.com/marinasundstrom/PointOfSale). This was an exercise in software architecture and building distributed apps.

Later in the same year (2021), I created the projects "Accounting" (later ["Finance"](https://github.com/marinasundstrom/finance-app)) and ["Product Catalog"](https://github.com/marinasundstrom/product-catalog). Then, the ["TimeReport"](https://github.com/marinasundstrom/TimeReport) module - project management. These modules were later brought together into "YourBrand". From there they continued to evolve.

In 2022, I forked "YourBrand" and decided to focus on building an e-commerce system - calling the project ["eShop"](https://github.com/marinasundstrom/eShop). I copied modules to a a new solution. Built the actual e-shop site. This project involved the Product Catalog, Order system, Customer relations, and Inventory.

In the summer of 2023, I decided to start over with my e-shop system - the [new "YourBrand"](https://github.com/marinasundstrom/yourbrand_new_old). My objective was top build it with .NET 9 and modern Blazor, and to make it cloud-ready. I rewrote a few of the services, and I learned a lot about Azure from tinkering with that. But as the project progressed it felt like it was never going to be on par with the previous "eShop" project anytime soon. It would also get expensive to run in the cloud.

So in 2024, despite thinking that I would never make any changes to the "YourBrand" codebase, I started to merge the modules back from the other solutions - from "eShop" and the new "YourBrand". And that made much more sense than to rewrite everything - especially when there was a lot that I loved about this code base - like the shell and module experience. Not to forget the Finance modules.

This solution contains all these modules, plus a lot of improvements.

I have always focused on having a solid developer experience. It should be easy to get up and running with no installs or additional configuration. This solution has the potential of running in the cloud and locally equally well.

Earlier versions used the experimental Tye orchestrator, but that has been changed to Docker Compose for dependencies.

## Screenshots

### Consultant Management ("Showroom")

<a href="/Screenshots/Showroom - Profile.png">
<img src="/Screenshots/Showroom - Profile.png" height="250"  alt="Showroom - Profile"  /></a>

Profile

<a href="/Screenshots/Showroom - Consultants.png">
<img src="/Screenshots/Showroom - Consultants.png" height="250"  alt="Showroom - Consultants"  /></a>

Gallery

### Project Manager ("Time Report")

<a href="/Screenshots/Time Report.png">
<img src="/Screenshots/Time Report.png" height="250"  alt="TimeReport"  /></a>

Timesheet

<a href="/Screenshots/Time Report - Projects.png">
<img src="/Screenshots/Time Report - Projects.png" height="250"  alt="Project"  /></a>

Projects

### Products ("Catalog")

<a href="/Screenshots/Catalog - Products.png">
<img src="/Screenshots/Catalog - Products.png" height="250"  alt="Products"  /></a>

Products

## Architecture

<a href="/docs/architecture/overview.png">
<img src="/docs/architecture/overview.png" height="450"  alt="Overview"  /></a>

<p>Architectural Overview</p>

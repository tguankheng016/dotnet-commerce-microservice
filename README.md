## Project Overview

> This is a practical and imaginary eCommerce system built with `.NET 8` as the backend and `React` and `Angular` for the frontend. The system is designed using `Microservices Architecture`, `Vertical Slice Architecture`, and `Clean Architecture` principles. The goal is to demonstrate a scalable, maintainable, and testable approach for building modern eCommerce applications.

This project is extended from [dotnet-commerce-monolith](https://github.com/tguankheng016/dotnet-commerce-monolith) repository.

Other versions of this project are available in these repositories:

- [https://github.com/tguankheng016/dotnet-commerce-monolith](https://github.com/tguankheng016/dotnet-commerce-monolith)
- [https://github.com/tguankheng016/golang-ecommerce-microservice](https://github.com/tguankheng016/golang-ecommerce-microservice)
- [https://github.com/tguankheng016/golang-ecommerce-monolith](https://github.com/tguankheng016/golang-ecommerce-monolith)

OAuth project used:

- [https://github.com/tguankheng016/openiddict-oauth](https://github.com/tguankheng016/openiddict-oauth)

For more details on frontend projects

- [Angular Admin Portal](https://github.com/tguankheng016/dotnet-commerce-microservice/blob/main/apps/angular/README.md)
- [React EShop](https://github.com/tguankheng016/dotnet-commerce-microservice/blob/main/apps/react/README.md)

# Table of Contents

- [The Goals of This Project](#the-goals-of-this-project)
- [Plan](#plan)
- [Technologies Used](#technologies-used)
- [The Domain and Bounded Context - Service Boundary](#the-domain-and-bounded-context---service-boundary)
- [Quick Start](#quick-start)

## Goals Of This Project

- ✅ Using `Microservices` and `Vertical Slice Architecture` as a high level architecture
- ✅ Using `Event Driven Architecture` with `RabbitMQ` as Message Broker on top of `MassTransit` library
- ✅ Using `Inbox` and `Outbox` pattern of `MassTransit` to ensure `Exactly once Delivery` and `At Least One Delivery`
- ✅ Using `gRPC` for `internal communication` between microservices
- ✅ Using `MediatR` library for `CQRS` implementation.
- ✅ Using `PostgresQL` as the relational database.
- ✅ Using `MongoDB` as the NoSQL database.
- ✅ Using `xUnit` for unit testing and `NSubstitute` for mocking dependencies.
- ✅ Using `TestContainers` and `xUnit` for integration testing.
- ✅ Using `Minimal APIs` for handling requests.
- ✅ Using `Fluent Validation` as `Validation Pipeline Behaviour` of MediatR.
- ✅ Using `EF Core` to manage code first migration.
- ✅ Using `YARP` reverse proxy as API Gateway.
- ✅ Using `OpenTelemetry` and `Jaeger` for distributed tracing.
- ✅ Using `OpenIddict` for authentication based on OpenID-Connect and OAuth2.
- ✅ Using `Angular` to build admin facing application.
- ✅ Using `React` to build consumer facing application.
- ✅ Using `Azure Key Vault` to manage the secrets.
- ✅ Using `Github Actions` as CI/CD pipeline.
- ✅ Using `AWS EC2` for hosting.

## Plan

> This project is a work in progress, new features will be added over time.

| Feature          | Status       |
| ---------------- | ------------ |
| API Gateway      | Completed ✔️ |
| Identity Service | Completed ✔️ |
| Product Service  | Completed ✔️ |
| Cart Service     | Completed ✔️ |
| Admin Portal     | Completed ✔️ |
| EShop            | Completed ✔️ |

## Technologies Used

- **[`.NET 8`](https://github.com/dotnet/aspnetcore)** - The latest stable version of .NET for building high-performance applications.

- **[`MVC Versioning API`](https://github.com/microsoft/aspnet-api-versioning)** - Set of libraries which add service API versioning to ASP.NET Web API, OData with ASP.NET Web API, and ASP.NET Core.

- **[`EF Core`](https://github.com/dotnet/efcore)** - Modern object-relational mapper for .NET that enables LINQ queries, change tracking, updates, and database schema migrations.

- **[`AspNetCore OpenApi`](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/aspnetcore-openapi?view=aspnetcore-9.0&tabs=visual-studio#configure-openapi-document-generation)** - Provides built-in support for OpenAPI document generation in ASP.NET Core.

- **[`Masstransit`](https://github.com/MassTransit/MassTransit)** - Distributed Application Framework for .NET.

- **[`MediatR`](https://github.com/jbogard/MediatR)** - About
  Simple, unambitious mediator implementation in .NET

- **[`FluentValidation`](https://github.com/FluentValidation/FluentValidation)** - Popular .NET validation library for building strongly-typed validation rules.

- **[`Swagger UI`](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)** - Swagger tools for documenting API's built on ASP.NET Core.

- **[`Serilog`](https://github.com/serilog/serilog)** - Simple .NET logging with fully-structured events

- **[`Polly`](https://github.com/App-vNext/Polly)** - Polly is a .NET resilience and transient-fault-handling library that allows developers to express policies such as Retry, Circuit Breaker, Timeout, Bulkhead Isolation, and Fallback in a fluent and thread-safe manner.

- **[`Scrutor`](https://github.com/khellang/Scrutor)** - Assembly scanning and decoration extensions for Microsoft.Extensions.DependencyInjection

- **[`OpenIddict`](https://github.com/openiddict/openiddict-core)** - Flexible and versatile OAuth 2.0/OpenID Connect stack for .NET.

- **[`Opentelemetry-dotnet`](https://github.com/open-telemetry/opentelemetry-dotnet)** - The OpenTelemetry .NET Client

- **[`Jaeger`](https://github.com/jaegertracing/jaeger)** - About
  CNCF Jaeger, a Distributed Tracing Platform

- **[`EasyCaching`](https://github.com/dotnetcore/EasyCaching)** - Open source caching library that contains basic usages and some advanced usages of caching which can help us to handle caching more easier.

- **[`Redis`](https://github.com/redis/redis)** - Redis is an in-memory database that persists on disk. The data model is key-value, but many different kind of values are supported: Strings, Lists, Sets, Sorted Sets, Hashes, Streams, HyperLogLogs, Bitmaps.

- **[`Mapperly`](https://github.com/riok/mapperly)** - A .NET source generator for generating object mappings. No runtime reflection.

- **[`NSubstitute`](https://github.com/nsubstitute/NSubstitute)** - A friendly substitute for .NET mocking libraries.

- **[`Yarp`](https://github.com/microsoft/reverse-proxy)** - Reverse proxy toolkit for building fast proxy servers in .NET.

- **[`MongoDB.Driver`](https://github.com/mongodb/mongo-csharp-driver)** - .NET Driver for MongoDB.

- **[`gRPC-dotnet`](https://github.com/grpc/grpc-dotnet)** - gRPC functionality for .NET.

- **[`RabbitMQ`](https://github.com/rabbitmq)** - Reliable and mature messaging and streaming broker, which is easy to deploy on cloud environments, on-premises, and on your local machine

- **[`xUnit`](https://github.com/xunit/xunit)** - A free, open source, community-focused unit testing tool for the .NET Framework.

- **[`Bogus`](https://github.com/bchavez/Bogus)** - A simple fake data generator for C#, F#, and VB.NET. Based on and ported from the famed faker.js.

- **[`Testcontainers`](https://github.com/testcontainers/testcontainers-dotnet)** - Testcontainers for .NET is a library to support tests with throwaway instances of Docker containers.

## The Domain And Bounded Context - Service Boundary

- `Identity Service` - The Identity Service is a bounded context responsible for user authentication and authorization. It handles user creation along with assigning roles and permissions through .NET Core Identity and JWT-based authentication and authorization.

- `Product Service` - The Product Service is a bounded context responsible for handling CRUD operations related to product management.

- `Cart Service` - The Cart Service is a bounded context responsible for handling CRUD operations related to cart management.

## Quick Start

### Prerequisites

Before you begin, make sure you have the following installed:

- **.NET 8 SDK**
- **Docker**

Once you have .NET 8 and Docker installed, you can set up the project by following these steps:

Clone the repository:

```bash
git clone https://github.com/tguankheng016/dotnet-commerce-microservice.git
```

Run the development server:

```bash
cd deployments/docker-compose
docker-compose -f docker-compose.yml up -d
```

Once everything is set up, you should be able to access:

- Gateway: [http://localhost:5081](http://localhost:5081)
- Identity Service: [http://localhost:5196](http://localhost:5196)
- Product Service: [http://localhost:5285](http://localhost:5285)
- Cart Service: [http://localhost:5118](http://localhost:5118)

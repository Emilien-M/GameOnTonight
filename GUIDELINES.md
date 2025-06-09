# Implementation Guidelines for the GameOnTonight Project

This document serves as a guide for implementing the GameOnTonight project according to the desired architecture and patterns.

## Table of Contents

1. [Global Architecture](#global-architecture)
2. [Backend Guidelines](#backend-guidelines)
   - [Patterns and Principles](#patterns-and-principles-backend)
   - [File and Folder Structure](#file-and-folder-structure-backend)
   - [Naming Conventions](#naming-conventions-backend)
   - [Dependency Injection](#dependency-injection-backend)
   - [Namespaces](#namespaces-backend)
3. [PWA Guidelines](#pwa-guidelines)
   - [Coming Soon...](#coming-soon)

## Global Architecture

The project follows Clean Architecture with the following layers:
- **Domain**: Entities and interfaces (IRepository, IService)
- **Application**: Business logic with CQRS and Mediator
- **Infrastructure**: Concrete implementations (repositories, services)
- **API**: REST entry points
- **Blazor WASM**: User interface (PWA)

## Backend Guidelines

### Patterns and Principles (Backend)

#### 1. Repository Pattern
- Use the Repository pattern for data access
- Define interfaces in Domain and implement them in Infrastructure
- Each entity has its specific repository
- Repositories inherit from a generic base class Repository<TEntity>
- Repositories should not contain complex business logic

#### 2. Unit of Work Pattern
- Use a UnitOfWork middleware to automatically apply changes at the end of each HTTP request
- The UnitOfWork is responsible for coordinating transactions between repositories

#### 3. CQRS with Mediator
- Separate read operations (Queries) and write operations (Commands)
- Use the Mediator library for dispatching commands and queries
- Structure the code as follows:
  - Requests are C# records
  - Handlers are defined in the same file as the corresponding request
  - Commands generally return an ID or a boolean
  - Queries return ViewModels (never direct entities)

#### 4. User-based Security
- All business entities implement the IUserOwnedEntity interface
- Each entity belongs to a specific user via the UserId property
- Data access is filtered by user
- Use the CurrentUserService to retrieve the authenticated user

#### 5. Common Properties
- All entities inherit from BaseEntity which provides:
  - Id (int)
  - CreatedAt (DateTime)
  - UpdatedAt (DateTime?)
- CreatedAt and UpdatedAt properties are automatically managed by AuditableEntityInterceptor

#### 6. ViewModels and Input/Output Management
- Never directly expose entities in the API
- Use ViewModels (records) for API endpoint responses
- API controllers take Commands and Queries directly as input models
- Define these objects as C# records
- ViewModels should contain only the properties necessary for display
- ViewModels should include a constructor that takes the corresponding entity as a parameter and performs the mapping automatically

#### 7. Validation
- Input validation is done in Mediator handlers
- Verify authorizations and access rights in handlers
- Return appropriate exceptions in case of error

### File and Folder Structure (Backend)

#### For Entities
```
GameOnTonight.Domain/
  Entities/
    BaseEntity.cs
    UserOwnedEntity.cs
    [EntityName].cs
```

#### For Repositories
```
GameOnTonight.Domain/
  Repositories/
    IRepository.cs
    I[EntityName]Repository.cs

GameOnTonight.Infrastructure/
  Repositories/
    Repository.cs
    [EntityName]Repository.cs
```

#### For Application Layer (CQRS)
```
GameOnTonight.Application/
  [EntityName]s/
    ViewModels/
      [EntityName]ViewModel.cs
    Commands/
      Create[EntityName]Command.cs
      Update[EntityName]Command.cs
      Delete[EntityName]Command.cs
    Queries/
      Get[EntityName]ByIdQuery.cs
      GetAll[EntityName]sQuery.cs
      [OtherSpecificRequests]Query.cs
```

#### For API
```
GameOnTonight.Api/
  Controllers/
    [EntityName]sController.cs
```

### Naming Conventions (Backend)
- Use PascalCase for classes, interfaces, properties, and methods
- Use camelCase for local variables and parameters
- Prefix interfaces with "I"
- Suffix repository classes with "Repository"
- Suffix commands with "Command"
- Suffix queries with "Query"
- Suffix handlers with "Handler"
- Suffix ViewModels with "ViewModel"

### Dependency Injection (Backend)
- Register all services in extension methods (DependencyInjection.cs)
- Use scoped lifetime for repositories and services
- Use the [FromServices] attribute for injection in minimal endpoints

### Namespaces (Backend)
- GameOnTonight.Domain.*
- GameOnTonight.Application.*
- GameOnTonight.Infrastructure.*
- GameOnTonight.Api.*

## PWA Guidelines

### Coming Soon...

By following these guidelines, implementation will respect the desired architecture and patterns for the GameOnTonight project.

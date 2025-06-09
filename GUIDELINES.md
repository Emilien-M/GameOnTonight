# Implementation Guidelines for the GameOnTonight Project

This document serves as a guide for implementing the GameOnTonight project according to the desired architecture and patterns.

## Table of Contents

1. [Global Architecture](#global-architecture)
2. [Backend Guidelines](#backend-guidelines)
   - [Patterns and Principles](#patterns-and-principles-backend)
   - [Error Handling](#error-handling)
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
- Domain entities should validate their own business rules by using DomainError pattern

### Error Handling

#### 1. Global Error Handling
- All exceptions are caught by the `ErrorHandlingMiddleware` and transformed into standardized API responses
- Never handle exceptions manually in controllers, let the middleware handle them
- API responses follow a consistent error format using the `ErrorResponse` model

#### 2. Domain Validation
- Domain entities inherit from `BaseEntity` which provides error collection capabilities
- Entities validate their own state by adding domain errors via `AddDomainError` methods
- Use provided helper methods like `ValidateString` and `ValidateNumber` for common validations
- Complex validations involving multiple properties should be defined in dedicated validation methods
- Always validate domain invariants in setters or business method calls

#### 3. Domain Errors and Exceptions
- `DomainError`: Value object representing a business rule violation, stored in entities
- `DomainException`: Exception containing one or more DomainErrors, thrown at transaction boundaries
- Domain errors are caught and converted to HTTP 400 Bad Request responses
- Other exceptions are treated as HTTP 500 Internal Server Errors by default

#### 4. Error Persistance Prevention
- The UnitOfWork checks all tracked entities for domain errors before saving to database
- If domain errors are detected, a `DomainException` is thrown and changes are not persisted
- This ensures that invalid entities are never stored in the database

#### 5. Error Response Format
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Validation Error Title",
  "status": 400,
  "traceId": "c7a27145-8352-4267-9238-b3148ce41e32",
  "errors": {
    "propertyName1": ["Error message 1", "Error message 2"],
    "propertyName2": ["Error message 3"]
  }
}
```

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

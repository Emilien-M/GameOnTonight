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
    - [Code Quality Guidelines](#code-quality-guidelines)
    - [C# Coding Rules](#csharp-coding-rules)
    - [Code Organization](#code-organization)
3. [PWA Guidelines](#pwa-guidelines)
    - [Coming Soon...](#coming-soon)

## Global Architecture

The project follows Clean Architecture with the following layers:
- **Domain**: Entities and interfaces (IRepository, IService)
- **Application**: Business logic with CQRS and Mediator
- **Infrastructure**: Concrete implementations (repositories, services)
- **API**: REST entry points
- **MAUI App**: User interface (Mobile App)

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

### Code Quality Guidelines

#### 1. SOLID Principles
- **Single Responsibility Principle**: Limit each class/function to one well-defined task or area of concern
- **Open/Closed Principle**: Design extensible abstractions and avoid modifications to existing, tested code
- **Liskov Substitution Principle**: Ensure derived types preserve the behavior contracts of base types
- **Interface Segregation Principle**: Create focused, minimal interfaces with only the methods a client needs
- **Dependency Inversion Principle**: Inject dependencies and program to interfaces, not implementations

#### 2. Comments and Documentation
- Add comments only to explain 'why' something is done, not 'what' is being done
- Code should be self-documenting through clear variable names and function signatures
- Use comments only for:
    - Explaining complex algorithms or business logic
    - Documenting non-obvious edge cases or workarounds
    - Providing context for future developers when the solution isn't immediately intuitive
- Avoid redundant comments that merely restate what the code clearly shows

#### 3. General Principles
- Respect the DRY (Don't Repeat Yourself) principle to avoid code duplication
- Iterate on existing code before writing new functionality
- Check for similar existing functionality and analyze base classes before implementing
- Adopt a fail-fast approach instead of using fallback values or fallback applications
- If configuration does not work, throw exceptions and terminate the program
- Keep files short (fewer than 300 lines)
- When adding logging to classes, focus on logging errors only

### C# Coding Rules

#### 1. Data Structures and Collections
- Do not use Enumerable methods on indexable collections; use the collection directly
- Prefer comparing 'Count' to 0 rather than using 'Any()' for both clarity and performance

#### 2. Object-Oriented Design
- Do not use marker interfaces or base classes
- Make methods static if they don't use class attributes
- Use 'await using' instead of 'using' whenever possible

#### 3. Exception Handling
- Use ArgumentNullException.ThrowIfNull and similar methods whenever possible
- Either log the exception and handle it, OR rethrow it with contextual information
- Don't combine exception logging and rethrowing without adding context

#### 4. Naming and Methods
- Use "Async" suffix in names of methods that return an awaitable type
- Inherit from ControllerBase instead of Controller for API controllers
- A method should have a maximum of 7 parameters

#### 5. Date and Time
- Always set the "DateTimeKind" when creating new "DateTime" instances

#### 6. Code Quality
- Try to fix Sonar & Roslyn errors and warnings if any
- Do not test private methods using reflection; test functionality through public methods instead

### Code Organization

#### 1. Step-Down Rule
- Write code that follows the step-down rule:
    - Each function should be followed by functions at the next level of abstraction
    - High-level functions appear first, with their implementation details following
    - A reader can read the code from top to bottom, with each function introducing concepts used by those that follow
    - Functions should be organized in descending order of abstraction
    - Ensure clear separation between different levels of abstraction

## MAUI Guidelines

### Coming Soon...

By following these guidelines, implementation will respect the desired architecture and patterns for the GameOnTonight project.

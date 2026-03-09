# Corvel.ToDo.Implementation

## Rules

@../../rules/common/coding-style.md
@../../rules/common/logging.md
@../../rules/common/patterns.md
@../../rules/csharp/coding-style.md
@../../rules/csharp/domain.md
@../../rules/csharp/services.md

## Module Purpose

Implements business logic and service interfaces defined in Abstractions. Contains service implementations, validation logic, and DI registration extensions.

## Key Contents

- Service implementations (e.g., `ToDoService : IToDoService`)
- FluentValidation validators
- DI registration extension methods (`Add{Feature}`)
- Business rules enforcement

## Dependency Constraints

- **Allowed**: Corvel.ToDo.Abstractions, Corvel.ToDo.Common
- **Forbidden**: Must NOT reference Repository, Web.Core, Web.Server, or any Web.* project

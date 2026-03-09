# Corvel.ToDo.Abstractions

## Rules

@../../rules/common/coding-style.md
@../../rules/common/patterns.md
@../../rules/csharp/coding-style.md
@../../rules/csharp/domain.md

## Module Purpose

Defines contracts and lightweight abstractions for the solution. Contains persistence-ignorant domain models, service and repository interfaces, custom exceptions, and value objects.

## Key Contents

- Domain models (no ORM attributes or navigation properties)
- Service interfaces (e.g., `IToDoService`)
- Repository interfaces (e.g., `IToDoRepository`)
- Request/response records
- Custom exception types (e.g., `NotFoundException`)
- Domain enumerations and value objects

## Dependency Constraints

- **Allowed**: Corvel.ToDo.Common
- **Forbidden**: Must NOT reference Implementation, Repository, or any Web.* project

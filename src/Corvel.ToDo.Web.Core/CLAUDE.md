# Corvel.ToDo.Web.Core

## Rules

@../../rules/common/coding-style.md
@../../rules/common/logging.md
@../../rules/common/patterns.md
@../../rules/common/security.md
@../../rules/csharp/coding-style.md
@../../rules/csharp/services.md
@../../rules/csharp/presentation.md
@../../rules/csharp/security.md

## Module Purpose

Core web functionality for the ASP.NET Core application. Contains API controllers, middleware, filters, and web-specific DI configuration.

## Key Contents

- API controllers (e.g., `ToDoController`)
- Custom middleware
- Action filters and result filters
- Model binders
- Web-specific DI registration

## Dependency Constraints

- **Allowed**: Corvel.ToDo.Abstractions, Corvel.ToDo.Implementation, Corvel.ToDo.Common, ASP.NET Core packages
- **Forbidden**: Must NOT reference Repository, Web.Server

# Corvel.ToDo.Web.Server

## Rules

@../../rules/common/coding-style.md
@../../rules/common/logging.md
@../../rules/common/patterns.md
@../../rules/common/security.md
@../../rules/csharp/coding-style.md
@../../rules/csharp/persistence.md
@../../rules/csharp/presentation.md
@../../rules/csharp/hosting.md
@../../rules/csharp/security.md

## Module Purpose

ASP.NET Core web host that serves the React SPA and bootstraps the application. Contains Program.cs, middleware pipeline setup, and application configuration.

## Key Contents

- `Program.cs` — application entry point and middleware pipeline
- DI container composition (calls `Add{Feature}` extensions from other assemblies)
- Global exception handler
- CORS, authentication, and authorization configuration
- Static file / SPA serving for React client

## Dependency Constraints

- **Allowed**: Corvel.ToDo.Web.Core, Corvel.ToDo.Implementation, Corvel.ToDo.Repository
- **Forbidden**: Must NOT reference Corvel.ToDo.Abstractions or Corvel.ToDo.Common directly (accessed transitively)
